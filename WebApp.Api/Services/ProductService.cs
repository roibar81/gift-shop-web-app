using WebApp.Api.Models;
using Microsoft.Extensions.Logging;

namespace WebApp.Api.Services;

public class ProductService
{
    private readonly List<Product> _products;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
        _products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Luxury Watch",
                Description = "Elegant timepiece perfect for any occasion",
                Price = 199.99m,
                ImageUrl = "https://picsum.photos/id/1/300/300",
                Category = "Accessories"
            },
            new Product
            {
                Id = 2,
                Name = "Scented Candle Set",
                Description = "Set of 3 premium scented candles",
                Price = 39.99m,
                ImageUrl = "https://picsum.photos/id/2/300/300",
                Category = "Home"
            },
            new Product
            {
                Id = 3,
                Name = "Gourmet Chocolate Box",
                Description = "Assorted premium chocolates",
                Price = 29.99m,
                ImageUrl = "https://picsum.photos/id/3/300/300",
                Category = "Food"
            },
            new Product
            {
                Id = 4,
                Name = "Leather Wallet",
                Description = "Handcrafted genuine leather wallet",
                Price = 49.99m,
                ImageUrl = "https://picsum.photos/id/4/300/300",
                Category = "Accessories"
            },
            new Product
            {
                Id = 5,
                Name = "Essential Oil Diffuser",
                Description = "Modern aromatherapy diffuser with LED lights",
                Price = 45.99m,
                ImageUrl = "https://picsum.photos/id/5/300/300",
                Category = "Home"
            }
        };
    }

    public IEnumerable<Product> GetAllProducts()
    {
        _logger.LogInformation($"Returning {_products.Count} products");
        return _products;
    }
    
    public Product? GetProductById(int id)
    {
        _logger.LogInformation($"Getting product with id: {id}");
        return _products.FirstOrDefault(p => p.Id == id);
    }
    
    public IEnumerable<Product> GetProductsByCategory(string category)
    {
        _logger.LogInformation($"Getting products in category: {category}");
        return _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }
} 