using WebApp.Api.Models;

namespace WebApp.Api.Services;

public class CartService
{
    private readonly Dictionary<string, List<CartItem>> _carts = new();
    private readonly ProductService _productService;

    public CartService(ProductService productService)
    {
        _productService = productService;
    }

    public List<CartItem> GetCart(string userId)
    {
        return _carts.GetValueOrDefault(userId) ?? new List<CartItem>();
    }

    public CartItem? AddToCart(string userId, int productId, int quantity = 1)
    {
        var product = _productService.GetProductById(productId);
        if (product == null) return null;

        if (!_carts.ContainsKey(userId))
        {
            _carts[userId] = new List<CartItem>();
        }

        var cart = _carts[userId];
        var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var newItem = new CartItem
            {
                Id = cart.Count + 1,
                ProductId = productId,
                Quantity = quantity,
                UserId = userId,
                Product = product
            };
            cart.Add(newItem);
            return newItem;
        }

        return existingItem;
    }

    public bool RemoveFromCart(string userId, int productId)
    {
        if (!_carts.ContainsKey(userId)) return false;

        var cart = _carts[userId];
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return false;

        return cart.Remove(item);
    }

    public bool UpdateQuantity(string userId, int productId, int quantity)
    {
        if (!_carts.ContainsKey(userId)) return false;

        var cart = _carts[userId];
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return false;

        if (quantity <= 0)
        {
            return cart.Remove(item);
        }

        item.Quantity = quantity;
        return true;
    }

    public void ClearCart(string userId)
    {
        if (_carts.ContainsKey(userId))
        {
            _carts[userId].Clear();
        }
    }
} 