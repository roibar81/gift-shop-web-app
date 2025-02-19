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
            // Electronics
            new Product { Id = 1, Name = "Wireless Earbuds", Description = "High-quality wireless earbuds with noise cancellation", Price = 129.99m, Category = "Electronics", ImageUrl = "https://picsum.photos/id/1/300/300" },
            new Product { Id = 2, Name = "Smart Watch", Description = "Feature-rich smartwatch with fitness tracking", Price = 199.99m, Category = "Electronics", ImageUrl = "https://picsum.photos/id/2/300/300" },
            new Product { Id = 3, Name = "Portable Speaker", Description = "Waterproof Bluetooth speaker with great sound", Price = 79.99m, Category = "Electronics", ImageUrl = "https://picsum.photos/id/3/300/300" },
            new Product { Id = 4, Name = "Digital Camera", Description = "Entry-level digital camera with HD video", Price = 299.99m, Category = "Electronics", ImageUrl = "https://picsum.photos/id/4/300/300" },
            new Product { Id = 5, Name = "Gaming Mouse", Description = "RGB gaming mouse with programmable buttons", Price = 59.99m, Category = "Electronics", ImageUrl = "https://picsum.photos/id/5/300/300" },

            // Books & Games
            new Product { Id = 6, Name = "Classic Novel Collection", Description = "Set of 5 classic literature books", Price = 49.99m, Category = "Books", ImageUrl = "https://picsum.photos/id/6/300/300" },
            new Product { Id = 7, Name = "Board Game Bundle", Description = "Collection of popular family board games", Price = 89.99m, Category = "Games", ImageUrl = "https://picsum.photos/id/7/300/300" },
            new Product { Id = 8, Name = "Puzzle Set", Description = "3 challenging 1000-piece puzzles", Price = 34.99m, Category = "Games", ImageUrl = "https://picsum.photos/id/8/300/300" },
            new Product { Id = 9, Name = "Art Book", Description = "Coffee table book featuring modern art", Price = 59.99m, Category = "Books", ImageUrl = "https://picsum.photos/id/9/300/300" },
            new Product { Id = 10, Name = "Strategy Game", Description = "Complex strategy board game", Price = 44.99m, Category = "Games", ImageUrl = "https://picsum.photos/id/10/300/300" },

            // Home & Living
            new Product { Id = 11, Name = "Scented Candle Set", Description = "Set of 3 premium scented candles", Price = 39.99m, Category = "Home", ImageUrl = "https://picsum.photos/id/11/300/300" },
            new Product { Id = 12, Name = "Throw Blanket", Description = "Soft and cozy decorative blanket", Price = 49.99m, Category = "Home", ImageUrl = "https://picsum.photos/id/12/300/300" },
            new Product { Id = 13, Name = "Photo Frame Set", Description = "Set of 5 matching photo frames", Price = 29.99m, Category = "Home", ImageUrl = "https://picsum.photos/id/13/300/300" },
            new Product { Id = 14, Name = "Plant Kit", Description = "Indoor plant starter kit with pots", Price = 44.99m, Category = "Home", ImageUrl = "https://picsum.photos/id/14/300/300" },
            new Product { Id = 15, Name = "Wall Clock", Description = "Modern decorative wall clock", Price = 34.99m, Category = "Home", ImageUrl = "https://picsum.photos/id/15/300/300" },

            // Fashion & Accessories
            new Product { Id = 16, Name = "Leather Wallet", Description = "Handcrafted genuine leather wallet", Price = 49.99m, Category = "Accessories", ImageUrl = "https://picsum.photos/id/16/300/300" },
            new Product { Id = 17, Name = "Sunglasses", Description = "Designer sunglasses with UV protection", Price = 89.99m, Category = "Accessories", ImageUrl = "https://picsum.photos/id/17/300/300" },
            new Product { Id = 18, Name = "Watch Set", Description = "Set of 2 casual watches", Price = 129.99m, Category = "Accessories", ImageUrl = "https://picsum.photos/id/18/300/300" },
            new Product { Id = 19, Name = "Scarf Collection", Description = "Set of 3 seasonal scarves", Price = 39.99m, Category = "Accessories", ImageUrl = "https://picsum.photos/id/19/300/300" },
            new Product { Id = 20, Name = "Jewelry Box", Description = "Wooden jewelry organizer box", Price = 54.99m, Category = "Accessories", ImageUrl = "https://picsum.photos/id/20/300/300" },

            // Sports & Outdoors
            new Product { Id = 21, Name = "Yoga Mat Set", Description = "Premium yoga mat with accessories", Price = 44.99m, Category = "Sports", ImageUrl = "https://picsum.photos/id/21/300/300" },
            new Product { Id = 22, Name = "Camping Kit", Description = "Basic camping gear set", Price = 149.99m, Category = "Sports", ImageUrl = "https://picsum.photos/id/22/300/300" },
            new Product { Id = 23, Name = "Sports Watch", Description = "Water-resistant sports watch", Price = 79.99m, Category = "Sports", ImageUrl = "https://picsum.photos/id/23/300/300" },
            new Product { Id = 24, Name = "Fitness Band", Description = "Activity tracker with heart rate monitor", Price = 59.99m, Category = "Sports", ImageUrl = "https://picsum.photos/id/24/300/300" },
            new Product { Id = 25, Name = "Tennis Set", Description = "Tennis racket pair with balls", Price = 89.99m, Category = "Sports", ImageUrl = "https://picsum.photos/id/25/300/300" },

            // Food & Beverages
            new Product { Id = 26, Name = "Gourmet Coffee Set", Description = "Premium coffee beans collection", Price = 49.99m, Category = "Food", ImageUrl = "https://picsum.photos/id/26/300/300" },
            new Product { Id = 27, Name = "Tea Collection", Description = "Luxury tea assortment box", Price = 39.99m, Category = "Food", ImageUrl = "https://picsum.photos/id/27/300/300" },
            new Product { Id = 28, Name = "Chocolate Box", Description = "Assorted premium chocolates", Price = 29.99m, Category = "Food", ImageUrl = "https://picsum.photos/id/28/300/300" },
            new Product { Id = 29, Name = "Wine Set", Description = "Selected wine duo with accessories", Price = 89.99m, Category = "Food", ImageUrl = "https://picsum.photos/id/29/300/300" },
            new Product { Id = 30, Name = "Snack Hamper", Description = "Curated gourmet snack collection", Price = 69.99m, Category = "Food", ImageUrl = "https://picsum.photos/id/30/300/300" },

            // Art & Craft
            new Product { Id = 31, Name = "Paint Set", Description = "Professional acrylic paint set", Price = 59.99m, Category = "Art", ImageUrl = "https://picsum.photos/id/31/300/300" },
            new Product { Id = 32, Name = "Sketchbook Bundle", Description = "Quality sketchbooks with pencils", Price = 39.99m, Category = "Art", ImageUrl = "https://picsum.photos/id/32/300/300" },
            new Product { Id = 33, Name = "Craft Kit", Description = "DIY craft project kit", Price = 44.99m, Category = "Art", ImageUrl = "https://picsum.photos/id/33/300/300" },
            new Product { Id = 34, Name = "Calligraphy Set", Description = "Beginner's calligraphy kit", Price = 34.99m, Category = "Art", ImageUrl = "https://picsum.photos/id/34/300/300" },
            new Product { Id = 35, Name = "Pottery Tools", Description = "Essential pottery tool set", Price = 49.99m, Category = "Art", ImageUrl = "https://picsum.photos/id/35/300/300" },

            // Toys
            new Product { Id = 36, Name = "Building Blocks", Description = "Creative building block set", Price = 39.99m, Category = "Toys", ImageUrl = "https://picsum.photos/id/36/300/300" },
            new Product { Id = 37, Name = "Remote Car", Description = "RC car with controller", Price = 49.99m, Category = "Toys", ImageUrl = "https://picsum.photos/id/37/300/300" },
            new Product { Id = 38, Name = "Doll Set", Description = "Collection of fashion dolls", Price = 34.99m, Category = "Toys", ImageUrl = "https://picsum.photos/id/38/300/300" },
            new Product { Id = 39, Name = "Science Kit", Description = "Educational science experiment kit", Price = 44.99m, Category = "Toys", ImageUrl = "https://picsum.photos/id/39/300/300" },
            new Product { Id = 40, Name = "Musical Toy", Description = "Interactive musical instrument toy", Price = 29.99m, Category = "Toys", ImageUrl = "https://picsum.photos/id/40/300/300" }
        };
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        _logger.LogInformation($"Returning {_products.Count} products");
        return await Task.FromResult(_products);
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        _logger.LogInformation($"Getting product with id: {id}");
        return await Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
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