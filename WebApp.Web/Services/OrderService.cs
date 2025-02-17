using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApp.Web.Models;
using System.Text.Json;

namespace WebApp.Web.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly CartService _cartService;

        public OrderService(HttpClient httpClient, CartService cartService)
        {
            _httpClient = httpClient;
            _cartService = cartService;
        }

        public async Task<Order> CreateOrderAsync(string userId, string shippingAddress, string paymentMethod)
        {
            try
            {
                // Get cart items
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                if (cartItems == null || cartItems.Count == 0)
                {
                    throw new Exception("Cart is empty");
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = cartItems.Sum(item => (item.Product?.Price ?? 0) * item.Quantity),
                    Status = "Pending",
                    ShippingAddress = shippingAddress,
                    PaymentMethod = paymentMethod,
                    Items = cartItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product?.Price ?? 0,
                        Product = item.Product
                    }).ToList()
                };

                // Send order to API
                var response = await _httpClient.PostAsJsonAsync($"api/orders", order);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create order: {error}");
                }

                // Clear the cart after successful order
                await _cartService.ClearCartAsync();

                return await response.Content.ReadFromJsonAsync<Order>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                throw;
            }
        }

        public async Task<Order> GetOrderAsync(int orderId, string userId)
        {
            var response = await _httpClient.GetAsync($"api/orders/{orderId}?userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Order>();
            }
            return null;
        }
    }
} 