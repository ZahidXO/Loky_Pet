namespace LokyPet.Client.Services;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LokyPet.Application.DTOs;
using LokyPet.Infrastructure.Configuration;

public sealed class CatalogService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private List<ProductVariantDto>? _cache;
    private List<CategoryDto>? _categoriesCache;

    public CatalogService(SupabaseConfig config)
    {
        _http = new HttpClient();
        _baseUrl = $"{config.Url}/rest/v1";
        _http.DefaultRequestHeaders.TryAddWithoutValidation("apikey", config.AnonKey);
        _http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {config.AnonKey}");
    }

    public async Task<List<ProductVariantDto>> GetAllVariantsAsync()
    {
        if (_cache is not null) return _cache;
        
        var url = $"{_baseUrl}/product_variants?select=id,product_id,sku,slug,color_name,color_hex,price_mxn,compare_at_price_mxn,stock,is_out_of_stock,is_active,product:products(name,slug,description,category:categories(name,slug),images:product_images(storage_path,sort_order)),images:product_images(storage_path,sort_order)&is_active=eq.true&order=sku";
        var rows = await _http.GetFromJsonAsync<List<VariantRow>>(url, JsonOpts) ?? new();
        _cache = rows.Select(MapToDto).ToList();
        return _cache;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        if (_categoriesCache is not null) return _categoriesCache;
        
        var url = $"{_baseUrl}/categories?select=name,slug,sort_order&order=sort_order";
        _categoriesCache = await _http.GetFromJsonAsync<List<CategoryDto>>(url, JsonOpts) ?? new();
        return _categoriesCache;
    }

    public async Task<List<ProductVariantDto>> GetVariantsByCategoryAsync(string categorySlug)
    {
        var all = await GetAllVariantsAsync();
        return all.Where(v => v.CategorySlug.Equals(categorySlug, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<ProductVariantDto>> GetVariantsOnSaleAsync()
    {
        var all = await GetAllVariantsAsync();
        return all.Where(v => v.IsOnSale).ToList();
    }

    public async Task<ProductVariantDto?> GetVariantBySlugAsync(string slug)
    {
        var all = await GetAllVariantsAsync();
        return all.FirstOrDefault(v => v.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(Guid productId)
    {
        var all = await GetAllVariantsAsync();
        return all.Where(v => v.ProductId == productId).OrderBy(v => v.ColorName).ToList();
    }

    public async Task<List<ProductVariantDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return await GetAllVariantsAsync();
        var all = await GetAllVariantsAsync();
        var q = query.Trim().ToLowerInvariant();
        return all.Where(v => 
            v.ProductName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            v.ColorName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            v.Sku.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            v.CategoryName.Contains(q, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }

    public void InvalidateCache()
    {
        _cache = null;
        _categoriesCache = null;
    }

    // --- JSON mapping ---

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static ProductVariantDto MapToDto(VariantRow r)
    {
        // Try variant-level images first, then product-level images from the product join
        var image = r.Images?.OrderBy(i => i.SortOrder).FirstOrDefault()?.StoragePath
                    ?? r.Product?.Images?.OrderBy(i => i.SortOrder).FirstOrDefault()?.StoragePath
                    ?? GetDefaultImage(r.Product?.Slug);

        return new ProductVariantDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            Sku = r.Sku,
            Slug = r.Slug,
            ProductName = r.Product?.Name ?? "",
            CategoryName = r.Product?.Category?.Name ?? "",
            CategorySlug = r.Product?.Category?.Slug ?? "",
            Description = r.Product?.Description ?? "",
            ColorName = r.ColorName,
            ColorHex = r.ColorHex ?? Array.Empty<string>(),
            PriceMxn = r.PriceMxn,
            CompareAtPriceMxn = r.CompareAtPriceMxn,
            Stock = r.Stock,
            IsOutOfStock = r.IsOutOfStock,
            Image = image
        };
    }

    /// <summary>Fallback image by product slug (not category) so each toy gets its own image.</summary>
    private static string GetDefaultImage(string? productSlug) => productSlug switch
    {
        "cama" => "/images/products/cama.png",
        "cojin-rellenable" => "/images/products/cojin-rellenable.png",
        "pelota-de-tiras" => "/images/products/pelota-de-tiras.png",
        "raton-de-medias" => "/images/products/raton-de-medias.png",
        "cuerda-de-juego" => "/images/products/cuerda-de-juego.png",
        _ => "/images/products/pelota-de-tiras.png"
    };

    // Row classes for JSON deserialization
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
        public bool IsActive { get; set; }
        public ProductRow? Product { get; set; }
        public List<ImageRow>? Images { get; set; }
    }

    private sealed class ProductRow
    {
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string Description { get; set; } = "";
        public CategoryRow? Category { get; set; }
        public List<ImageRow>? Images { get; set; }
    }

    private sealed class CategoryRow
    {
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
    }

    private sealed class ImageRow
    {
        public string StoragePath { get; set; } = "";
        public int SortOrder { get; set; }
    }
}

public sealed class CategoryDto
{
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public int SortOrder { get; set; }
}
