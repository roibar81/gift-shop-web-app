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

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, CartService cartService, OrderService orderService)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7001/");
        _cartService = cartService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            _logger.LogInformation("Fetching products from API");
            var response = await _httpClient.GetAsync("api/products");
            _logger.LogInformation($"API Response Status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response Content: {content}");
                
                var products = JsonSerializer.Deserialize<List<Product>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation($"Deserialized {products?.Count ?? 0} products");
                return View(products);
            }
            else
            {
                _logger.LogError($"API returned non-success status code: {response.StatusCode}");
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API Error: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
        }
        
        _logger.LogWarning("Returning empty product list due to error");
        return View(new List<Product>());
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

    [HttpPost]
    public async Task<IActionResult> UpdateCartQuantity(int productId, int quantity)
    {
        try 
        {
            var success = await _cartService.UpdateQuantityAsync(productId, quantity);
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
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        try 
        {
            var success = await _cartService.RemoveFromCartAsync(productId);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
