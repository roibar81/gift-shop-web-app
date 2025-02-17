namespace WebApp.Web.Models;

public class GiftSuggestionRequest
{
    public decimal Budget { get; set; }
    public int Age { get; set; }
    public string Occasion { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string UseFrequency { get; set; } = string.Empty;
} 