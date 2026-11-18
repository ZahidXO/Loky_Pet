namespace LokyPet.Domain.Entities;

using LokyPet.Domain.ValueObjects;

public sealed class ProductVariant
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = "";
    public string Slug { get; set; } = "";
    public string ColorName { get; set; } = "";
    public string[] ColorHex { get; set; } = Array.Empty<string>();
    public decimal PriceMxn { get; set; }
    public decimal? CompareAtPriceMxn { get; set; }
    public int Stock { get; set; }
    public bool IsOutOfStock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Product? Product { get; set; }

    public bool IsOnSale => CompareAtPriceMxn.HasValue && CompareAtPriceMxn.Value > PriceMxn;
    public int DiscountPct => IsOnSale ? (int)Math.Round((1m - PriceMxn / CompareAtPriceMxn!.Value) * 100m) : 0;
    public decimal Savings => IsOnSale ? CompareAtPriceMxn!.Value - PriceMxn : 0m;
}
