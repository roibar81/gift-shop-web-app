using System.Text;
using System.Text.Json;
using WebApp.Web.Models;

namespace WebApp.Web.Services;

public class CartService
{
    private readonly HttpClient _httpClient;
    private readonly string _userId;

    public CartService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7001/");
        _userId = "user1"; // For demo purposes, we'll use a fixed user ID
    }

    public async Task<List<CartItem>> GetCartAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/cart/{_userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CartItem>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CartItem>();
        }
        catch
        {
            return new List<CartItem>();
        }
    }

    public async Task<CartItem?> AddToCartAsync(int productId, int quantity = 1)
    {
        try
        {
            var request = new { ProductId = productId, Quantity = quantity };
            var jsonRequest = JsonSerializer.Serialize(request);
            Console.WriteLine($"Sending request to API: {jsonRequest}");
            
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/cart/{_userId}/items", content);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response Status: {response.StatusCode}");
            Console.WriteLine($"API Response Content: {responseContent}");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error adding to cart: {responseContent}");
                return null;
            }
            
            return JsonSerializer.Deserialize<CartItem>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception adding to cart: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<bool> RemoveFromCartAsync(int productId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/cart/{_userId}/items/{productId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateQuantityAsync(int productId, int quantity)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/cart/{_userId}/items/{productId}", 
                new StringContent(JsonSerializer.Serialize(new { Quantity = quantity }), 
                Encoding.UTF8, "application/json"));
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating quantity: {ex.Message}");
            return false;
        }
    }

    public async Task ClearCartAsync()
    {
        try
        {
            await _httpClient.DeleteAsync($"api/cart/{_userId}");
        }
        catch
        {
            // Ignore any errors when clearing the cart
        }
    }

    public async Task<List<CartItem>> GetCartItemsAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/cart/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CartItem>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<CartItem>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting cart items: {ex.Message}");
        }
        return new List<CartItem>();
    }
} 