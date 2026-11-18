namespace LokyPet.Application.Interfaces;
using LokyPet.Application.DTOs;

public interface ICartService
{
    event Action? OnChange;
    Task<List<CartItemDto>> GetItemsAsync();
    Task AddItemAsync(CartItemDto item);
    Task UpdateQuantityAsync(string variantSlug, int quantity);
    Task RemoveItemAsync(string variantSlug);
    Task ClearAsync();
    Task<int> GetCountAsync();
    Task<decimal> GetSubtotalAsync();
}
