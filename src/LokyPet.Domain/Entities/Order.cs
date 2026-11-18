namespace LokyPet.Domain.Entities;

using LokyPet.Domain.Enums;

public sealed class Order
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public Guid? UserId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string ShippingAddress { get; set; } = "";
    public string ShippingCity { get; set; } = "";
    public string ShippingState { get; set; } = "";
    public string ShippingZipCode { get; set; } = "";
    public ShippingType ShippingType { get; set; }
    public string? ShippingCarrier { get; set; }
    public string? TrackingNumber { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Paid;
    public bool IsSimulated { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
