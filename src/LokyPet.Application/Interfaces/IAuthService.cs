namespace LokyPet.Application.Interfaces;
using LokyPet.Domain.Entities;

public interface IAuthService
{
    event Action? OnAuthStateChanged;
    Task<UserProfile?> GetCurrentUserAsync();
    Task<bool> IsAdminAsync();
    Task<bool> SignInAsync(string email, string password);
    Task<bool> SignUpAsync(string email, string password, string fullName);
    Task SignOutAsync();
    Task<bool> ResetPasswordAsync(string email);
    Task<UserProfile?> UpdateProfileAsync(UserProfile profile);
}
