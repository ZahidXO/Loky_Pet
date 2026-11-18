namespace LokyPet.Client.Components;

public sealed class DemoProduct
{
    public string Sku { get; init; } = "";
    public string Name { get; init; } = "";
    public string Slug { get; init; } = "";
    public string Category { get; init; } = "";
    public string CategorySlug { get; init; } = "";
    public string ColorName { get; init; } = "";
    public string[] ColorHex { get; init; } = Array.Empty<string>();
    public decimal PriceMxn { get; init; }
    public decimal? CompareAtPriceMxn { get; init; }
    public int Stock { get; init; } = 10;
    public string Image { get; init; } = "";
    public string Description { get; init; } = "";
    
    // All color variants of this product (for grouped display)
    public List<ColorVariantInfo> ColorVariants { get; init; } = new();

    public bool IsOnSale => CompareAtPriceMxn is > 0 && CompareAtPriceMxn > PriceMxn;
    public bool IsOutOfStock => Stock <= 0;
    public int DiscountPct => IsOnSale ? (int)Math.Round((1 - PriceMxn / CompareAtPriceMxn!.Value) * 100) : 0;
    public decimal Savings => IsOnSale ? CompareAtPriceMxn!.Value - PriceMxn : 0;
}

public sealed class ColorVariantInfo
{
    public string Slug { get; init; } = "";
    public string ColorName { get; init; } = "";
    public string[] ColorHex { get; init; } = Array.Empty<string>();
    public int Stock { get; init; }
}
