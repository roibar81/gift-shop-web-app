using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.Api.Data;
using WebApp.Api.Models;

namespace WebApp.Api.Services;

public class ProductService
{
    private readonly GiftShopContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(GiftShopContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        _logger.LogInformation("Fetching all products from database");
        return await _context.Products.Include(p => p.Category).ToListAsync();
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        _logger.LogInformation($"Getting product with id: {id} from database");
        return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    }

    public IEnumerable<Product> GetAllProducts()
    {
        _logger.LogInformation("Fetching all products from database");
        return _context.Products.Include(p => p.Category).ToList();
    }
    
    public Product? GetProductById(int id)
    {
        _logger.LogInformation($"Getting product with id: {id} from database");
        return _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
    }
    
    public IEnumerable<Product> GetProductsByCategory(string categoryName)
    {
        _logger.LogInformation($"Getting products in category: {categoryName} from database");
        return _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category.Name.ToLower() == categoryName.ToLower())
            .ToList();
    }

    public async Task SeedInitialDataAsync()
    {
        if (!await _context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Electronics" },
                new Category { Name = "Books" },
                new Category { Name = "Games" },
                new Category { Name = "Home" },
                new Category { Name = "Accessories" },
                new Category { Name = "Sports" },
                new Category { Name = "Food" },
                new Category { Name = "Art" },
                new Category { Name = "Toys" }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded initial category data");
        }

        if (!await _context.Products.AnyAsync())
        {
            var categories = await _context.Categories.ToDictionaryAsync(c => c.Name, c => c);
            
            var products = new List<Product>
            {
                // Electronics
                new Product { Name = "Wireless Earbuds", Description = "High-quality wireless earbuds with noise cancellation", Price = 129.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/1/300/300" },
                new Product { Name = "Smart Watch", Description = "Feature-rich smartwatch with fitness tracking", Price = 199.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/2/300/300" },
                new Product { Name = "Portable Speaker", Description = "Waterproof Bluetooth speaker with great sound", Price = 79.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/3/300/300" },
                new Product { Name = "Digital Camera", Description = "Entry-level digital camera with HD video", Price = 299.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/4/300/300" },
                new Product { Name = "Gaming Mouse", Description = "RGB gaming mouse with programmable buttons", Price = 59.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/5/300/300" },

                // Books & Games
                new Product { Name = "Classic Novel Collection", Description = "Set of 5 classic literature books", Price = 49.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/6/300/300" },
                new Product { Name = "Board Game Bundle", Description = "Collection of popular family board games", Price = 89.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/7/300/300" },
                new Product { Name = "Puzzle Set", Description = "3 challenging 1000-piece puzzles", Price = 34.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/8/300/300" },
                new Product { Name = "Art Book", Description = "Coffee table book featuring modern art", Price = 59.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/9/300/300" },
                new Product { Name = "Strategy Game", Description = "Complex strategy board game", Price = 44.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/10/300/300" },

                // Home & Living
                new Product { Name = "Scented Candle Set", Description = "Set of 3 premium scented candles", Price = 39.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/11/300/300" },
                new Product { Name = "Throw Blanket", Description = "Soft and cozy decorative blanket", Price = 49.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/12/300/300" },
                new Product { Name = "Photo Frame Set", Description = "Set of 5 matching photo frames", Price = 29.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/13/300/300" },
                new Product { Name = "Plant Kit", Description = "Indoor plant starter kit with pots", Price = 44.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/14/300/300" },
                new Product { Name = "Wall Clock", Description = "Modern decorative wall clock", Price = 34.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/15/300/300" },

                // Fashion & Accessories
                new Product { Name = "Leather Wallet", Description = "Handcrafted genuine leather wallet", Price = 49.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/16/300/300" },
                new Product { Name = "Sunglasses", Description = "Designer sunglasses with UV protection", Price = 89.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/17/300/300" },
                new Product { Name = "Watch Set", Description = "Set of 2 casual watches", Price = 129.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/18/300/300" },
                new Product { Name = "Scarf Collection", Description = "Set of 3 seasonal scarves", Price = 39.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/19/300/300" },
                new Product { Name = "Jewelry Box", Description = "Wooden jewelry organizer box", Price = 54.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/20/300/300" },

                // Sports & Outdoors
                new Product { Name = "Yoga Mat Set", Description = "Premium yoga mat with accessories", Price = 44.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/21/300/300" },
                new Product { Name = "Camping Kit", Description = "Basic camping gear set", Price = 149.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/22/300/300" },
                new Product { Name = "Sports Watch", Description = "Water-resistant sports watch", Price = 79.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/23/300/300" },
                new Product { Name = "Fitness Band", Description = "Activity tracker with heart rate monitor", Price = 59.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/24/300/300" },
                new Product { Name = "Tennis Set", Description = "Tennis racket pair with balls", Price = 89.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/25/300/300" },

                // Food & Beverages
                new Product { Name = "Gourmet Coffee Set", Description = "Premium coffee beans collection", Price = 49.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/26/300/300" },
                new Product { Name = "Tea Collection", Description = "Luxury tea assortment box", Price = 39.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/27/300/300" },
                new Product { Name = "Chocolate Box", Description = "Assorted premium chocolates", Price = 29.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/28/300/300" },
                new Product { Name = "Wine Set", Description = "Selected wine duo with accessories", Price = 89.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/29/300/300" },
                new Product { Name = "Snack Hamper", Description = "Curated gourmet snack collection", Price = 69.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/30/300/300" },

                // Art & Craft
                new Product { Name = "Paint Set", Description = "Professional acrylic paint set", Price = 59.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/31/300/300" },
                new Product { Name = "Sketchbook Bundle", Description = "Quality sketchbooks with pencils", Price = 39.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/32/300/300" },
                new Product { Name = "Craft Kit", Description = "DIY craft project kit", Price = 44.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/33/300/300" },
                new Product { Name = "Calligraphy Set", Description = "Beginner's calligraphy kit", Price = 34.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/34/300/300" },
                new Product { Name = "Pottery Tools", Description = "Essential pottery tool set", Price = 49.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/35/300/300" },

                // Toys
                new Product { Name = "Building Blocks", Description = "Creative building block set", Price = 39.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/36/300/300" },
                new Product { Name = "Remote Car", Description = "RC car with controller", Price = 49.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/37/300/300" },
                new Product { Name = "Doll Set", Description = "Collection of fashion dolls", Price = 34.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/38/300/300" },
                new Product { Name = "Science Kit", Description = "Educational science experiment kit", Price = 44.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/39/300/300" },
                new Product { Name = "Musical Toy", Description = "Interactive musical instrument toy", Price = 29.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/40/300/300" },

                // Additional Electronics
                new Product { Name = "Mechanical Keyboard", Description = "RGB mechanical gaming keyboard with custom switches", Price = 149.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/41/300/300" },
                new Product { Name = "Tablet", Description = "10-inch tablet with high-resolution display", Price = 299.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/42/300/300" },
                new Product { Name = "Power Bank", Description = "20000mAh portable charger with fast charging", Price = 49.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/43/300/300" },
                new Product { Name = "Webcam", Description = "1080p webcam with built-in microphone", Price = 79.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/44/300/300" },
                new Product { Name = "Gaming Headset", Description = "Surround sound gaming headset with noise cancellation", Price = 129.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/45/300/300" },

                // Additional Books
                new Product { Name = "Cookbook Collection", Description = "Set of international cuisine cookbooks", Price = 79.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/46/300/300" },
                new Product { Name = "Science Book Series", Description = "Collection of popular science books", Price = 89.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/47/300/300" },
                new Product { Name = "Photography Guide", Description = "Complete guide to digital photography", Price = 44.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/48/300/300" },
                new Product { Name = "History Collection", Description = "Illustrated world history book set", Price = 129.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/49/300/300" },
                new Product { Name = "Poetry Anthology", Description = "Collection of contemporary poetry", Price = 34.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/50/300/300" },

                // Additional Games
                new Product { Name = "Card Game Set", Description = "Collection of classic card games", Price = 29.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/51/300/300" },
                new Product { Name = "Mystery Board Game", Description = "Detective-themed board game", Price = 39.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/52/300/300" },
                new Product { Name = "Party Game Bundle", Description = "Set of fun party games", Price = 49.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/53/300/300" },
                new Product { Name = "Chess Set", Description = "Wooden chess set with storage", Price = 69.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/54/300/300" },
                new Product { Name = "Educational Games", Description = "Set of learning games for kids", Price = 44.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/55/300/300" },

                // Additional Home items
                new Product { Name = "Decorative Pillows", Description = "Set of 4 designer throw pillows", Price = 59.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/56/300/300" },
                new Product { Name = "Table Runner Set", Description = "Seasonal table runners collection", Price = 39.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/57/300/300" },
                new Product { Name = "Wall Art Set", Description = "Collection of modern wall prints", Price = 89.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/58/300/300" },
                new Product { Name = "Vase Collection", Description = "Set of 3 decorative vases", Price = 69.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/59/300/300" },
                new Product { Name = "Storage Baskets", Description = "Woven storage basket set", Price = 44.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/60/300/300" },

                // Additional Accessories
                new Product { Name = "Hair Accessories", Description = "Luxury hair accessory set", Price = 29.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/61/300/300" },
                new Product { Name = "Belt Collection", Description = "Set of leather belts", Price = 79.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/62/300/300" },
                new Product { Name = "Travel Set", Description = "Travel accessories collection", Price = 89.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/63/300/300" },
                new Product { Name = "Tie Collection", Description = "Set of silk ties", Price = 69.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/64/300/300" },
                new Product { Name = "Glove Set", Description = "Winter gloves collection", Price = 44.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/65/300/300" },

                // Additional Sports items
                new Product { Name = "Basketball Set", Description = "Basketball with pump and accessories", Price = 49.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/66/300/300" },
                new Product { Name = "Hiking Gear", Description = "Essential hiking equipment set", Price = 129.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/67/300/300" },
                new Product { Name = "Swimming Kit", Description = "Complete swimming gear set", Price = 69.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/68/300/300" },
                new Product { Name = "Exercise Mat Set", Description = "Exercise mat with accessories", Price = 39.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/69/300/300" },
                new Product { Name = "Sports Bag", Description = "Multi-purpose sports duffel bag", Price = 54.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/70/300/300" },

                // Additional Food items
                new Product { Name = "Spice Collection", Description = "Gourmet spice gift set", Price = 59.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/71/300/300" },
                new Product { Name = "Olive Oil Set", Description = "Premium olive oil collection", Price = 79.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/72/300/300" },
                new Product { Name = "Hot Sauce Kit", Description = "Artisanal hot sauce collection", Price = 44.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/73/300/300" },
                new Product { Name = "Cookie Set", Description = "Gourmet cookie assortment", Price = 34.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/74/300/300" },
                new Product { Name = "Honey Collection", Description = "Specialty honey gift set", Price = 49.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/75/300/300" },

                // Additional Art supplies
                new Product { Name = "Drawing Set", Description = "Professional drawing supplies kit", Price = 79.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/76/300/300" },
                new Product { Name = "Watercolor Kit", Description = "Complete watercolor painting set", Price = 89.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/77/300/300" },
                new Product { Name = "Sculpture Tools", Description = "Clay sculpting tool set", Price = 54.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/78/300/300" },
                new Product { Name = "Art Paper Collection", Description = "Mixed media paper set", Price = 39.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/79/300/300" },
                new Product { Name = "Canvas Set", Description = "Assorted canvas pack", Price = 44.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/80/300/300" },

                // Additional Toys
                new Product { Name = "Educational Toy Set", Description = "STEM learning toy collection", Price = 69.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/81/300/300" },
                new Product { Name = "Plush Collection", Description = "Set of soft plush animals", Price = 49.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/82/300/300" },
                new Product { Name = "Construction Set", Description = "Advanced building block set", Price = 79.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/83/300/300" },
                new Product { Name = "Art Toy Kit", Description = "Creative art toy set", Price = 39.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/84/300/300" },
                new Product { Name = "Outdoor Toy Set", Description = "Collection of outdoor play toys", Price = 54.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/85/300/300" },

                // Additional Mixed Products
                new Product { Name = "Smart Home Hub", Description = "Voice-controlled home automation hub", Price = 199.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/86/300/300" },
                new Product { Name = "Manga Collection", Description = "Popular manga series box set", Price = 89.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/87/300/300" },
                new Product { Name = "Virtual Reality Game", Description = "Immersive VR gaming experience", Price = 59.99m, Category = categories["Games"], ImageUrl = "https://picsum.photos/id/88/300/300" },
                new Product { Name = "Smart Light Set", Description = "Color-changing smart LED bulbs", Price = 79.99m, Category = categories["Home"], ImageUrl = "https://picsum.photos/id/89/300/300" },
                new Product { Name = "Designer Backpack", Description = "Premium waterproof backpack", Price = 129.99m, Category = categories["Accessories"], ImageUrl = "https://picsum.photos/id/90/300/300" },
                new Product { Name = "Golf Set", Description = "Beginner's golf club set", Price = 299.99m, Category = categories["Sports"], ImageUrl = "https://picsum.photos/id/91/300/300" },
                new Product { Name = "Wine Tasting Kit", Description = "Wine appreciation set with guide", Price = 149.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/92/300/300" },
                new Product { Name = "Digital Art Tablet", Description = "Professional drawing tablet", Price = 199.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/93/300/300" },
                new Product { Name = "Robot Kit", Description = "Educational robotics building set", Price = 89.99m, Category = categories["Toys"], ImageUrl = "https://picsum.photos/id/94/300/300" },
                new Product { Name = "Wireless Charger", Description = "Fast wireless charging pad", Price = 39.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/95/300/300" },
                new Product { Name = "Gardening Book", Description = "Complete guide to home gardening", Price = 34.99m, Category = categories["Books"], ImageUrl = "https://picsum.photos/id/96/300/300" },
                new Product { Name = "Smart Scale", Description = "WiFi-connected body composition scale", Price = 69.99m, Category = categories["Electronics"], ImageUrl = "https://picsum.photos/id/97/300/300" },
                new Product { Name = "Premium Tea Set", Description = "Traditional ceramic tea ceremony set", Price = 119.99m, Category = categories["Food"], ImageUrl = "https://picsum.photos/id/98/300/300" },
                new Product { Name = "3D Printing Pen", Description = "Creative 3D drawing tool", Price = 79.99m, Category = categories["Art"], ImageUrl = "https://picsum.photos/id/99/300/300" }
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded initial product data");
        }
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPaginatedProductsAsync(int page = 1, int pageSize = 12)
    {
        _logger.LogInformation($"Fetching page {page} with {pageSize} products per page");
        var query = _context.Products.Include(p => p.Category);
        
        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPaginatedProductsByCategoryAsync(string categoryName, int page = 1, int pageSize = 12)
    {
        _logger.LogInformation($"Fetching page {page} with {pageSize} products for category: {categoryName}");
        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category.Name.ToLower() == categoryName.ToLower());
        
        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }
} 