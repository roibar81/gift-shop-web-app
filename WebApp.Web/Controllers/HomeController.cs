using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Web.Models;
using System.Text.Json;
using WebApp.Web.Services;

namespace WebApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;
    private readonly CartService _cartService;
    private readonly OrderService _orderService;
    private readonly ProductService _productService;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, CartService cartService, OrderService orderService, ProductService productService)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7001/");
        _cartService = cartService;
        _orderService = orderService;
        _productService = productService;
    }

    public async Task<IActionResult> Index(string? category = null, int page = 1)
    {
        try
        {
            var pageSize = 12;
            var (products, totalCount) = await _productService.GetPaginatedProductsAsync(category, page, pageSize);
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Category = category;

            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
            return View(new List<Product>());
        }
    }

    public async Task<IActionResult> Cart()
    {
        try
        {
            _logger.LogInformation("Fetching cart items");
            var cartItems = await _cartService.GetCartAsync();
            _logger.LogInformation($"Retrieved {cartItems.Count} cart items");
            return View(cartItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cart items");
            return View(new List<CartItem>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        _logger.LogInformation($"Received AddToCart request - ProductId: {request.ProductId}, Quantity: {request.Quantity}");
        
        var item = await _cartService.AddToCartAsync(request.ProductId, request.Quantity);
        if (item == null)
        {
            _logger.LogError("Failed to add item to cart");
            return Json(new { success = false, message = "Failed to add item to cart" });
        }

        var cart = await _cartService.GetCartAsync();
        return Json(new { success = true, cartCount = cart.Sum(i => i.Quantity) });
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartQuantityRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCartQuantity([FromBody] UpdateCartQuantityRequest request)
    {
        try 
        {
            var success = await _cartService.UpdateQuantityAsync(request.ProductId, request.Quantity);
            if (!success)
                return Json(new { success = false, message = "Failed to update quantity" });

            var cart = await _cartService.GetCartAsync();
            return Json(new { 
                success = true, 
                cartCount = cart.Sum(i => i.Quantity),
                newTotal = cart.Sum(i => (i.Product?.Price ?? 0) * i.Quantity)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating cart quantity: {ex.Message}");
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
    {
        try 
        {
            var success = await _cartService.RemoveFromCartAsync(request.ProductId);
            if (!success)
                return Json(new { success = false, message = "Failed to remove item" });

            var cart = await _cartService.GetCartAsync();
            return Json(new { 
                success = true, 
                cartCount = cart.Sum(i => i.Quantity),
                newTotal = cart.Sum(i => (i.Product?.Price ?? 0) * i.Quantity)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing item from cart: {ex.Message}");
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    public class RemoveFromCartRequest
    {
        public int ProductId { get; set; }
    }

    public IActionResult Chat()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Checkout()
    {
        var cartItems = await _cartService.GetCartItemsAsync("user1");
        if (cartItems == null || !cartItems.Any())
        {
            return RedirectToAction("Cart");
        }
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync("user1", request.ShippingAddress, request.PaymentMethod);
            return Json(order);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error placing order: {ex.Message}");
            return BadRequest(new { error = "Failed to place order" });
        }
    }

    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var order = await _orderService.GetOrderAsync(id, "user1");
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    public class PlaceOrderRequest
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> GetGiftSuggestions([FromBody] GiftSuggestionRequest request)
    {
        try
        {
            // Get all products
            var response = await _httpClient.GetAsync("api/products");
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to get products");
            }

            var content = await response.Content.ReadAsStringAsync();
            var allProducts = JsonSerializer.Deserialize<List<Product>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Product>();

            // Filter products based on budget and age
            var suggestions = allProducts
                .Where(p => p.Price <= request.Budget)
                .Select(p => new
                {
                    Product = p,
                    AgeScore = CalculateAgeScore(p.Category, request.Age)
                })
                .Where(p => p.AgeScore > 0) // Filter out products with 0 age score
                .OrderByDescending(p => p.AgeScore) // Sort by age relevance
                .Select(p => p.Product)
                .ToList();

            return Json(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting gift suggestions: {ex.Message}");
            return BadRequest("Failed to get gift suggestions");
        }
    }

    private double CalculateAgeScore(string category, int age)
    {
        // Age ranges and their appropriate categories
        if (age < 12) // Kids
        {
            switch (category.ToLower())
            {
                case "toys":
                case "games":
                    return 1.0;
                case "books":
                    return 0.8;
                case "accessories":
                    return 0.4;
                default:
                    return 0.2;
            }
        }
        else if (age < 20) // Teenagers
        {
            switch (category.ToLower())
            {
                case "accessories":
                case "electronics":
                    return 1.0;
                case "games":
                case "sports":
                    return 0.8;
                case "books":
                    return 0.6;
                default:
                    return 0.4;
            }
        }
        else if (age < 30) // Young Adults
        {
            switch (category.ToLower())
            {
                case "electronics":
                case "accessories":
                    return 1.0;
                case "home":
                    return 0.8;
                case "food":
                    return 0.7;
                default:
                    return 0.5;
            }
        }
        else // Adults
        {
            switch (category.ToLower())
            {
                case "home":
                case "accessories":
                    return 1.0;
                case "food":
                    return 0.9;
                case "electronics":
                    return 0.7;
                default:
                    return 0.6;
            }
        }
    }

    public async Task<IActionResult> Product(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (product != null)
                {
                    return View(product);
                }
            }
            
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting product details: {ex.Message}");
            return NotFound();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
