namespace LokyPet.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Category? Category { get; set; }
    public List<ProductVariant> Variants { get; set; } = new();
    public List<ProductImage> Images { get; set; } = new();
}
