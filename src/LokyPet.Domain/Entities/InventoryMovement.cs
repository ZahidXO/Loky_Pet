namespace LokyPet.Domain.Entities;

public sealed class InventoryMovement
{
    public Guid Id { get; set; }
    public Guid VariantId { get; set; }
    public int Delta { get; set; }
    public string Reason { get; set; } = "";
    public Guid? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
