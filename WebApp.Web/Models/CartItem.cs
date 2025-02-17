namespace WebApp.Web.Models;

public class CartItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Product? Product { get; set; }
} 