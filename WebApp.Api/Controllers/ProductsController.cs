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
    public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts([FromQuery] string? category = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        try
        {
            var (products, totalCount) = category == null || category.ToLower() == "all"
                ? await _productService.GetPaginatedProductsAsync(page, pageSize)
                : await _productService.GetPaginatedProductsByCategoryAsync(category, page, pageSize);

            var result = new PaginatedResult<ProductDto>
            {
                Items = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Category = p.Category.Name
                }),
                TotalCount = totalCount
            };

            return Ok(result);
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

    [HttpPost("import-from-platzi")]
    public async Task<IActionResult> ImportFromPlatzi()
    {
        try
        {
            await _productService.ImportProductsFromPlatziApiAsync();
            return Ok("Products imported successfully from Platzi API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing products from Platzi API");
            return StatusCode(500, "Error importing products from Platzi API");
        }
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportProducts()
    {
        try
        {
            var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "CsvExporter", "products.csv");
            if (!System.IO.File.Exists(csvPath))
            {
                return BadRequest("CSV file not found");
            }

            await _productService.ImportProductsFromCsvAsync(csvPath);
            return Ok("Products imported successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing products");
            return StatusCode(500, "Error importing products");
        }
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        try
        {
            var products = await _productService.GetProductsAsync();
            var categories = products
                .Select(p => p.Category.Name)
                .Distinct()
                .OrderBy(c => c);
            
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, "Internal server error");
        }
    }
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
} 