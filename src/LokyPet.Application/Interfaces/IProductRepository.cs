namespace LokyPet.Application.Interfaces;
using LokyPet.Domain.Entities;

public interface IProductRepository
{
    Task<List<ProductVariant>> GetAllVariantsAsync(bool includeInactive = false);
    Task<List<ProductVariant>> GetVariantsByCategoryAsync(string categorySlug);
    Task<List<ProductVariant>> GetVariantsOnSaleAsync();
    Task<ProductVariant?> GetVariantBySlugAsync(string slug);
    Task<List<ProductVariant>> GetVariantsByProductIdAsync(Guid productId);
    Task<List<Category>> GetCategoriesAsync();
    Task<List<ProductVariant>> SearchVariantsAsync(string query);
}
