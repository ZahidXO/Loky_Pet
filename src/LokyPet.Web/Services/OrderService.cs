namespace LokyPet.Client.Services;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LokyPet.Application.Interfaces;
using LokyPet.Domain.Entities;
using LokyPet.Domain.Enums;
using LokyPet.Infrastructure.Configuration;

/// <summary>
/// Implementación de IOrderRepository usando PostgREST de Supabase.
/// Para Fase 6 solo se implementa GetOrdersByUserAsync (historial del cliente).
/// Los métodos de admin se completan en Fase 7.
/// </summary>
public sealed class OrderService : IOrderRepository
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string _anonKey;
    private readonly IAuthService _auth;

    public OrderService(SupabaseConfig config, IAuthService auth)
    {
        _http = new HttpClient();
        _baseUrl = $"{config.Url}/rest/v1";
        _anonKey = config.AnonKey;
        _auth = auth;
    }

    public async Task<List<Order>> GetOrdersByUserAsync(Guid userId)
    {
        try
        {
            var url = $"{_baseUrl}/orders?user_id=eq.{userId}&select=*,items:order_items(*)&order=created_at.desc";
            var request = CreateRequest(HttpMethod.Get, url);
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new();

            var rows = await response.Content.ReadFromJsonAsync<List<OrderRow>>(JsonOpts);
            return rows?.Select(MapToOrder).ToList() ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
    {
        try
        {
            var url = $"{_baseUrl}/orders?order_number=eq.{Uri.EscapeDataString(orderNumber)}&select=*,items:order_items(*)";
            var request = CreateRequest(HttpMethod.Get, url);
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var rows = await response.Content.ReadFromJsonAsync<List<OrderRow>>(JsonOpts);
            var row = rows?.FirstOrDefault();
            return row is not null ? MapToOrder(row) : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        try
        {
            var url = $"{_baseUrl}/orders?id=eq.{id}&select=*,items:order_items(*)";
            var request = CreateRequest(HttpMethod.Get, url);
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var rows = await response.Content.ReadFromJsonAsync<List<OrderRow>>(JsonOpts);
            var row = rows?.FirstOrDefault();
            return row is not null ? MapToOrder(row) : null;
        }
        catch
        {
            return null;
        }
    }

    // Admin methods - implemented in Phase 7
    public async Task<Order> CreateOrderAsync(Order order)
    {
        try
        {
            // 1. POST the order header to /orders with Prefer: return=representation
            var orderUrl = $"{_baseUrl}/orders";
            var orderReq = CreateRequest(HttpMethod.Post, orderUrl);
            orderReq.Headers.TryAddWithoutValidation("Prefer", "return=representation");
            orderReq.Content = JsonContent.Create(new
            {
                order_number = order.OrderNumber,
                user_id = order.UserId,
                customer_name = order.CustomerName,
                customer_email = order.CustomerEmail,
                customer_phone = order.CustomerPhone,
                shipping_address = order.ShippingAddress,
                shipping_city = order.ShippingCity,
                shipping_state = order.ShippingState,
                shipping_zip_code = order.ShippingZipCode,
                shipping_type = order.ShippingType == ShippingType.Local ? "local" : "nacional",
                shipping_carrier = order.ShippingCarrier,
                tracking_number = order.TrackingNumber,
                subtotal = order.Subtotal,
                discount_total = order.DiscountTotal,
                shipping_cost = order.ShippingCost,
                total = order.Total,
                status = "paid",
                is_simulated = order.IsSimulated
            }, options: JsonOpts);

            var orderResp = await _http.SendAsync(orderReq);
            orderResp.EnsureSuccessStatusCode();

            // PostgREST returns an array even with Prefer: return=representation
            var createdRows = await orderResp.Content.ReadFromJsonAsync<List<OrderRow>>(JsonOpts);
            var createdRow = createdRows?.FirstOrDefault()
                ?? throw new InvalidOperationException("No order row returned from PostgREST.");

            var createdOrder = MapToOrder(createdRow);

            // 2. POST order items to /order_items
            if (order.Items is { Count: > 0 })
            {
                var itemsUrl = $"{_baseUrl}/order_items";
                var itemPayload = order.Items.Select(i => new
                {
                    order_id = createdOrder.Id,
                    variant_id = i.VariantId,
                    sku = i.Sku,
                    name = i.Name,
                    color = i.Color,
                    unit_price = i.UnitPrice,
                    compare_at_price = i.CompareAtPrice,
                    quantity = i.Quantity,
                    line_total = i.LineTotal
                }).ToList();

                var itemsReq = CreateRequest(HttpMethod.Post, itemsUrl);
                itemsReq.Headers.TryAddWithoutValidation("Prefer", "return=representation");
                itemsReq.Content = JsonContent.Create(itemPayload, options: JsonOpts);

                var itemsResp = await _http.SendAsync(itemsReq);
                itemsResp.EnsureSuccessStatusCode();

                var createdItems = await itemsResp.Content.ReadFromJsonAsync<List<OrderItemRow>>(JsonOpts);
                if (createdItems is not null)
                {
                    createdOrder.Items = createdItems.Select(i => new OrderItem
                    {
                        Id = i.Id,
                        OrderId = i.OrderId,
                        VariantId = i.VariantId,
                        Sku = i.Sku ?? "",
                        Name = i.Name ?? "",
                        Color = i.Color ?? "",
                        UnitPrice = i.UnitPrice,
                        CompareAtPrice = i.CompareAtPrice,
                        Quantity = i.Quantity,
                        LineTotal = i.LineTotal
                    }).ToList();
                }
            }

            return createdOrder;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create order: {ex.Message}", ex);
        }
    }
    public Task<List<Order>> GetAllOrdersAsync() => throw new NotImplementedException("Phase 7");
    public Task UpdateOrderStatusAsync(Guid orderId, string status, string? trackingNumber = null) => throw new NotImplementedException("Phase 7");

    // ── Helpers ──

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.TryAddWithoutValidation("apikey", _anonKey);
        var token = (_auth as LokyPet.Client.Services.AuthService)?.AccessToken;
        req.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token ?? _anonKey}");
        return req;
    }

    private static Order MapToOrder(OrderRow r)
    {
        var order = new Order
        {
            Id = r.Id,
            OrderNumber = r.OrderNumber ?? "",
            UserId = r.UserId,
            CustomerName = r.CustomerName ?? "",
            CustomerEmail = r.CustomerEmail ?? "",
            CustomerPhone = r.CustomerPhone ?? "",
            ShippingAddress = r.ShippingAddress ?? "",
            ShippingCity = r.ShippingCity ?? "",
            ShippingState = r.ShippingState ?? "",
            ShippingZipCode = r.ShippingZipCode ?? "",
            ShippingType = r.ShippingType == "local" ? ShippingType.Local : ShippingType.Nacional,
            ShippingCarrier = r.ShippingCarrier,
            TrackingNumber = r.TrackingNumber,
            Subtotal = r.Subtotal,
            DiscountTotal = r.DiscountTotal,
            ShippingCost = r.ShippingCost,
            Total = r.Total,
            Status = ParseStatus(r.Status),
            IsSimulated = r.IsSimulated,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
        };

        if (r.Items is not null)
        {
            order.Items = r.Items.Select(i => new OrderItem
            {
                Id = i.Id,
                OrderId = i.OrderId,
                VariantId = i.VariantId,
                Sku = i.Sku ?? "",
                Name = i.Name ?? "",
                Color = i.Color ?? "",
                UnitPrice = i.UnitPrice,
                CompareAtPrice = i.CompareAtPrice,
                Quantity = i.Quantity,
                LineTotal = i.LineTotal
            }).ToList();
        }

        return order;
    }

    private static OrderStatus ParseStatus(string? status) => status?.ToLowerInvariant() switch
    {
        "paid" or "pagado" => OrderStatus.Paid,
        "preparing" or "en preparación" or "en preparacion" => OrderStatus.Preparing,
        "shipped" or "enviado" => OrderStatus.Shipped,
        "delivered" or "entregado" => OrderStatus.Delivered,
        "cancelled" or "cancelado" => OrderStatus.Cancelled,
        _ => OrderStatus.Paid
    };

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // ── Row classes ──

    private sealed class OrderRow
    {
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public Guid? UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingState { get; set; }
        public string? ShippingZipCode { get; set; }
        public string? ShippingType { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? TrackingNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string? Status { get; set; }
        public bool IsSimulated { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemRow>? Items { get; set; }
    }

    private sealed class OrderItemRow
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid VariantId { get; set; }
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
