namespace LokyPet.Domain.Entities;

using LokyPet.Domain.Enums;

public sealed class UserProfile
{
    public Guid Id { get; set; }  // matches auth.users.id
    public string FullName { get; set; } = "";
    public string? Email { get; set; }    // from auth.users (GoTrue)
    public string? Phone { get; set; }
    public string? DefaultAddress { get; set; }  // JSON string
    public string? AvatarKey { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
