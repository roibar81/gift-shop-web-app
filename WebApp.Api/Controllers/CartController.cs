using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;
using WebApp.Api.Services;
using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WebApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(CartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    [HttpGet("{userId}")]
    public IActionResult GetCart(string userId)
    {
        _logger.LogInformation($"Getting cart for user: {userId}");
        var cart = _cartService.GetCart(userId);
        _logger.LogInformation($"Retrieved {cart.Count} items for user: {userId}");
        return Ok(cart);
    }

    [HttpPost("{userId}/items")]
    public IActionResult AddToCart(string userId, [FromBody] CartItemRequest request)
    {
        _logger.LogInformation($"Adding item to cart - UserId: {userId}, ProductId: {request.ProductId}, Quantity: {request.Quantity}");
        
        if (request == null)
        {
            _logger.LogWarning("Request body is null");
            return BadRequest("Request body is required");
        }

        var item = _cartService.AddToCart(userId, request.ProductId, request.Quantity);
        if (item == null)
        {
            _logger.LogWarning($"Failed to add product {request.ProductId} to cart");
            return NotFound("Product not found");
        }

        _logger.LogInformation($"Successfully added item to cart for user: {userId}");
        return Ok(item);
    }

    [HttpDelete("{userId}/items/{productId}")]
    public IActionResult RemoveFromCart(string userId, int productId)
    {
        _logger.LogInformation($"Removing item from cart - UserId: {userId}, ProductId: {productId}");
        var result = _cartService.RemoveFromCart(userId, productId);
        if (!result)
        {
            _logger.LogWarning($"Failed to remove product {productId} from cart");
            return NotFound();
        }
        
        _logger.LogInformation($"Successfully removed item from cart for user: {userId}");
        return Ok();
    }

    [HttpPut("{userId}/items/{productId}")]
    public IActionResult UpdateQuantity(string userId, int productId, [FromBody] UpdateQuantityRequest request)
    {
        _logger.LogInformation($"Updating quantity - UserId: {userId}, ProductId: {productId}, Quantity: {request.Quantity}");
        
        if (request == null || request.Quantity <= 0)
        {
            _logger.LogWarning("Invalid quantity");
            return BadRequest("Invalid quantity");
        }

        var result = _cartService.UpdateQuantity(userId, productId, request.Quantity);
        if (!result)
        {
            _logger.LogWarning($"Failed to update quantity for product {productId}");
            return NotFound();
        }
        
        _logger.LogInformation($"Successfully updated quantity for user: {userId}");
        return Ok();
    }

    [HttpDelete("{userId}")]
    public IActionResult ClearCart(string userId)
    {
        _logger.LogInformation($"Clearing cart for user: {userId}");
        _cartService.ClearCart(userId);
        _logger.LogInformation($"Successfully cleared cart for user: {userId}");
        return Ok();
    }
}

public class CartItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateQuantityRequest
{
    public int Quantity { get; set; }
} 