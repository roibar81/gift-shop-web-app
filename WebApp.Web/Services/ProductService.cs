using System.Text.Json;
using WebApp.Web.Models;

namespace WebApp.Web.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7001/");
        _logger = logger;
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPaginatedProductsAsync(string? category = null, int page = 1, int pageSize = 12)
    {
        try
        {
            var url = $"api/products?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(category) && category.ToLower() != "all")
            {
                url += $"&category={category}";
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<PaginatedResult<Product>>(content, options);
            
            return (result?.Items ?? Enumerable.Empty<Product>(), result?.TotalCount ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching paginated products");
            return (Enumerable.Empty<Product>(), 0);
        }
    }
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
} 