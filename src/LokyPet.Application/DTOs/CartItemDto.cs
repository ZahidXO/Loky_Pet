namespace LokyPet.Application.DTOs;

public sealed class CartItemDto
{
    public Guid VariantId { get; set; }
    public string VariantSlug { get; set; } = "";
    public string Sku { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string ColorName { get; set; } = "";
    public string[] ColorHex { get; set; } = Array.Empty<string>();
    public decimal UnitPrice { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public string Image { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public int MaxStock { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
    public bool IsOnSale => CompareAtPrice.HasValue && CompareAtPrice.Value > UnitPrice;
}
