namespace LokyPet.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid VariantId { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
