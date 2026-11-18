namespace LokyPet.Infrastructure.Repositories;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LokyPet.Application.DTOs;
using LokyPet.Application.Interfaces;
using LokyPet.Domain.Entities;
using LokyPet.Infrastructure.Configuration;

public sealed class SupabaseProductRepository : IProductRepository
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public SupabaseProductRepository(HttpClient http, SupabaseConfig config)
    {
        _http = http;
        _baseUrl = $"{config.Url}/rest/v1";
        _http.DefaultRequestHeaders.TryAddWithoutValidation("apikey", config.AnonKey);
        _http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {config.AnonKey}");
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var response = await _http.GetFromJsonAsync<List<CategoryRow>>(
            $"{_baseUrl}/categories?select=*&order=sort_order",
            _jsonOptions) ?? new();
        return response.Select(r => new Category
        {
            Id = r.Id, Name = r.Name, Slug = r.Slug, SortOrder = r.SortOrder
        }).ToList();
    }

    public async Task<List<ProductVariant>> GetAllVariantsAsync(bool includeInactive = false)
    {
        var filter = includeInactive ? "" : "&is_active=eq.true";
        var url = $"{_baseUrl}/product_variants?select=*,product:products(*,category:categories(*)),images:product_images(*)&order=sku{filter}";
        var rows = await _http.GetFromJsonAsync<List<VariantRow>>(url, _jsonOptions) ?? new();
        return rows.Select(MapVariant).ToList();
    }

    public async Task<List<ProductVariant>> GetVariantsByCategoryAsync(string categorySlug)
    {
        var url = $"{_baseUrl}/product_variants?select=*,product:products!inner(*,category:categories!inner(*)),images:product_images(*)&product.category.slug=eq.{categorySlug}&is_active=eq.true&order=sku";
        var rows = await _http.GetFromJsonAsync<List<VariantRow>>(url, _jsonOptions) ?? new();
        return rows.Select(MapVariant).ToList();
    }

    public async Task<List<ProductVariant>> GetVariantsOnSaleAsync()
    {
        var url = $"{_baseUrl}/product_variants?select=*,product:products(*,category:categories(*)),images:product_images(*)&compare_at_price_mxn=not.is.null&is_active=eq.true&order=sku";
        var rows = await _http.GetFromJsonAsync<List<VariantRow>>(url, _jsonOptions) ?? new();
        return rows.Where(r => r.CompareAtPriceMxn.HasValue && r.CompareAtPriceMxn > r.PriceMxn).Select(MapVariant).ToList();
    }

    public async Task<ProductVariant?> GetVariantBySlugAsync(string slug)
    {
        var url = $"{_baseUrl}/product_variants?select=*,product:products(*,category:categories(*)),images:product_images(*)&slug=eq.{slug}&limit=1";
        var rows = await _http.GetFromJsonAsync<List<VariantRow>>(url, _jsonOptions) ?? new();
        return rows.FirstOrDefault() is { } r ? MapVariant(r) : null;
    }

    public async Task<List<ProductVariant>> GetVariantsByProductIdAsync(Guid productId)
    {
        var url = $"{_baseUrl}/product_variants?select=*,product:products(*,category:categories(*))&product_id=eq.{productId}&is_active=eq.true&order=color_name";
        var rows = await _http.GetFromJsonAsync<List<VariantRow>>(url, _jsonOptions) ?? new();
        return rows.Select(MapVariant).ToList();
    }

    public async Task<List<ProductVariant>> SearchVariantsAsync(string query)
    {
        var encoded = Uri.EscapeDataString($"%{query}%");
        var url = $"{_baseUrl}/product_variants?select=*,product:products(*,category:categories(*)),images:product_images(*)&or=(sku.ilike.{encoded},color_name.ilike.{encoded},slug.ilike.{encoded})&is_active=eq.true&order=sku";
        var rows = await _http.GetFromJsonAsync<List<VariantRow>>(url, _jsonOptions) ?? new();
        return rows.Select(MapVariant).ToList();
    }

    // JSON mapping helpers
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static ProductVariant MapVariant(VariantRow r)
    {
        var v = new ProductVariant
        {
            Id = r.Id,
            ProductId = r.ProductId,
            Sku = r.Sku,
            Slug = r.Slug,
            ColorName = r.ColorName,
            ColorHex = r.ColorHex ?? Array.Empty<string>(),
            PriceMxn = r.PriceMxn,
            CompareAtPriceMxn = r.CompareAtPriceMxn,
            Stock = r.Stock,
            IsOutOfStock = r.IsOutOfStock,
            IsActive = r.IsActive,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
        if (r.Product is { } p)
        {
            v.Product = new Product
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Description,
                IsActive = p.IsActive
            };
            if (p.Category is { } c)
            {
                v.Product.Category = new Category
                {
                    Id = c.Id, Name = c.Name, Slug = c.Slug, SortOrder = c.SortOrder
                };
            }
        }
        return v;
    }

    // Row classes for JSON deserialization (snake_case from PostgreSQL)
    private sealed class CategoryRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public int SortOrder { get; set; }
    }

    private sealed class ProductRow
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; }
        public CategoryRow? Category { get; set; }
    }

    private sealed class VariantRow
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Sku { get; set; } = "";
        public string Slug { get; set; } = "";
        public string ColorName { get; set; } = "";
        public string[]? ColorHex { get; set; }
        public decimal PriceMxn { get; set; }
        public decimal? CompareAtPriceMxn { get; set; }
        public int Stock { get; set; }
        public bool IsOutOfStock { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ProductRow? Product { get; set; }
    }
}
