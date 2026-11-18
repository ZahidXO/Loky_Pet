namespace LokyPet.Domain.Entities;

public sealed class ProductImage
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? VariantId { get; set; }
    public string StoragePath { get; set; } = "";
    public int SortOrder { get; set; }
    public string AltText { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
