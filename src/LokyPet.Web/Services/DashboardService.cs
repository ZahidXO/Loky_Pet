namespace LokyPet.Client.Services;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LokyPet.Application.Interfaces;
using LokyPet.Infrastructure.Configuration;

/// <summary>
/// Service para obtener métricas del dashboard desde Supabase PostgREST.
/// Requiere rol admin (RLS). Usa el token JWT del usuario logueado.
/// </summary>
public sealed class DashboardService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string _anonKey;
    private readonly IAuthService _auth;

    public DashboardService(SupabaseConfig config, IAuthService auth)
    {
        _http = new HttpClient();
        _baseUrl = $"{config.Url}/rest/v1";
        _anonKey = config.AnonKey;
        _auth = auth;
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.TryAddWithoutValidation("apikey", _anonKey);
        var token = (_auth as AuthService)?.AccessToken;
        req.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token ?? _anonKey}");
        return req;
    }

    // ── Orders ──
    public async Task<List<OrderRow>> GetAllOrdersAsync()
    {
        var url = $"{_baseUrl}/orders?select=*,items:order_items(*)&order=created_at.desc";
        var req = CreateRequest(HttpMethod.Get, url);
        var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode) return new();
        return await resp.Content.ReadFromJsonAsync<List<OrderRow>>(JsonOpts) ?? new();
    }

    // ── Variants (inventory) ──
    public async Task<List<VariantRow>> GetAllVariantsAsync()
    {
        var url = $"{_baseUrl}/product_variants?select=*,product:products(name,slug,category:categories(name,slug))&order=sku";
        var req = CreateRequest(HttpMethod.Get, url);
        var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode) return new();
        return await resp.Content.ReadFromJsonAsync<List<VariantRow>>(JsonOpts) ?? new();
    }

    // ── Messages ──
    public async Task<List<MessageRow>> GetAllMessagesAsync()
    {
        var url = $"{_baseUrl}/contact_messages?order=created_at.desc";
        var req = CreateRequest(HttpMethod.Get, url);
        var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode) return new();
        return await resp.Content.ReadFromJsonAsync<List<MessageRow>>(JsonOpts) ?? new();
    }

    public async Task UpdateMessageStatusAsync(Guid id, string status)
    {
        var url = $"{_baseUrl}/contact_messages?id=eq.{id}";
        var request = CreateRequest(HttpMethod.Patch, url);
        request.Content = JsonContent.Create(new { status }, options: JsonOpts);
        request.Headers.TryAddWithoutValidation("Prefer", "return=minimal");
        await _http.SendAsync(request);
    }

    public async Task UpdateOrderStatusAsync(Guid id, string status, string? trackingNumber)
    {
        var url = $"{_baseUrl}/orders?id=eq.{id}";
        var payload = trackingNumber is not null
            ? (object)new { status, tracking_number = trackingNumber }
            : new { status };
        var request = CreateRequest(HttpMethod.Patch, url);
        request.Content = JsonContent.Create(payload, options: JsonOpts);
        request.Headers.TryAddWithoutValidation("Prefer", "return=minimal");
        await _http.SendAsync(request);
    }

    public async Task UpdateVariantStockAsync(Guid id, int stock, bool isOutOfStock)
    {
        var url = $"{_baseUrl}/product_variants?id=eq.{id}";
        var request = CreateRequest(HttpMethod.Patch, url);
        request.Content = JsonContent.Create(new { stock, is_out_of_stock = isOutOfStock }, options: JsonOpts);
        request.Headers.TryAddWithoutValidation("Prefer", "return=minimal");
        await _http.SendAsync(request);
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // ── Row DTOs ──
    public sealed class OrderRow
    {
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? ShippingType { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? TrackingNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemRow>? Items { get; set; }
    }

    public sealed class OrderItemRow
    {
        public Guid? VariantId { get; set; }
        public string? Sku { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    public sealed class VariantRow
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = "";
        public string Slug { get; set; } = "";
        public string ColorName { get; set; } = "";
        public decimal PriceMxn { get; set; }
        public decimal? CompareAtPriceMxn { get; set; }
        public int Stock { get; set; }
        public bool IsOutOfStock { get; set; }
        public bool IsActive { get; set; }
        public ProductRef? Product { get; set; }
    }

    public sealed class ProductRef
    {
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public CategoryRef? Category { get; set; }
    }

    public sealed class CategoryRef
    {
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
    }

    public sealed class MessageRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
        public string Status { get; set; } = "nuevo";
        public DateTime CreatedAt { get; set; }
    }
}
