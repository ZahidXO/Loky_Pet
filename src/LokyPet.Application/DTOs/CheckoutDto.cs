namespace LokyPet.Application.DTOs;

public sealed class CheckoutDto
{
    // Step 1: Shipping
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string ShippingAddress { get; set; } = "";
    public string ShippingCity { get; set; } = "";
    public string ShippingState { get; set; } = "";
    public string ShippingZipCode { get; set; } = "";
    public string ShippingType { get; set; } = "local"; // "local" or "nacional"
    
    // Step 2: Payment (simulated)
    public string PaymentMethod { get; set; } = "";
    
    // Cart items (resolved at checkout)
    public List<CartItemDto> Items { get; set; } = new();
}
