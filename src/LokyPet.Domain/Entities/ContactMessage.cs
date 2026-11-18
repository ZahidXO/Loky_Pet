namespace LokyPet.Domain.Entities;

public sealed class ContactMessage
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string Subject { get; set; } = "";
    public string Message { get; set; } = "";
    public string Status { get; set; } = "nuevo";  // nuevo, leido, respondido
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
