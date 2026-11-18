namespace LokyPet.Client.Services;

using Blazored.LocalStorage;
using LokyPet.Application.DTOs;
using LokyPet.Application.Interfaces;

public sealed class LocalStorageCartService : ICartService
{
    private const string StorageKey = "lokypet_cart";
    private readonly ILocalStorageService _storage;
    private List<CartItemDto>? _cache;

    public event Action? OnChange;

    public LocalStorageCartService(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async Task<List<CartItemDto>> GetItemsAsync()
    {
        _cache ??= await _storage.GetItemAsync<List<CartItemDto>>(StorageKey) ?? new();
        return _cache;
    }

    public async Task AddItemAsync(CartItemDto item)
    {
        var items = await GetItemsAsync();
        var existing = items.FirstOrDefault(i => i.VariantSlug == item.VariantSlug);
        if (existing is not null)
        {
            existing.Quantity = Math.Min(existing.Quantity + item.Quantity, existing.MaxStock);
        }
        else
        {
            items.Add(item);
        }
        await SaveAsync(items);
    }

    public async Task UpdateQuantityAsync(string variantSlug, int quantity)
    {
        var items = await GetItemsAsync();
        var item = items.FirstOrDefault(i => i.VariantSlug == variantSlug);
        if (item is not null)
        {
            item.Quantity = Math.Max(1, Math.Min(quantity, item.MaxStock));
            await SaveAsync(items);
        }
    }

    public async Task RemoveItemAsync(string variantSlug)
    {
        var items = await GetItemsAsync();
        items.RemoveAll(i => i.VariantSlug == variantSlug);
        await SaveAsync(items);
    }

    public async Task ClearAsync()
    {
        _cache = new();
        await _storage.SetItemAsync(StorageKey, _cache);
        OnChange?.Invoke();
    }

    public async Task<int> GetCountAsync()
    {
        var items = await GetItemsAsync();
        return items.Sum(i => i.Quantity);
    }

    public async Task<decimal> GetSubtotalAsync()
    {
        var items = await GetItemsAsync();
        return items.Sum(i => i.LineTotal);
    }

    private async Task SaveAsync(List<CartItemDto> items)
    {
        _cache = items;
        await _storage.SetItemAsync(StorageKey, items);
        OnChange?.Invoke();
    }
}
