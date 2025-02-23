using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;
using WebApp.Api.Services;

namespace WebApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<Product>>> GetProducts([FromQuery] string? category = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        try
        {
            var (products, totalCount) = category == null || category.ToLower() == "all" 
                ? await _productService.GetPaginatedProductsAsync(page, pageSize)
                : await _productService.GetPaginatedProductsByCategoryAsync(category, page, pageSize);

            return Ok(new PaginatedResult<Product>
            {
                Items = products,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpGet("category/{category}")]
    public IActionResult GetProductsByCategory(string category)
    {
        return Ok(_productService.GetProductsByCategory(category));
    }
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
} 