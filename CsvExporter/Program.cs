using System.Text;
using System.Text.Json;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
services.AddLogging(configure => configure.AddConsole());
services.AddHttpClient();

var serviceProvider = services.BuildServiceProvider();

try
{
    using (var scope = serviceProvider.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var client = httpClientFactory.CreateClient();

        // Fetch products from DummyJSON API
        logger.LogInformation("מתחיל לטעון מוצרים מ-DummyJSON");
        var response = await client.GetAsync("https://dummyjson.com/products?limit=100");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        logger.LogInformation($"התקבל JSON: {content.Substring(0, Math.Min(content.Length, 200))}...");

        var jsonResponse = JsonDocument.Parse(content);
        var products = jsonResponse.RootElement.GetProperty("products");

        // Create CSV content
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Id,Name,Description,Price,Category,ImageUrl,Brand,Rating,Stock");
        
        foreach (var product in products.EnumerateArray())
        {
            try
            {
                var id = product.GetProperty("id").GetInt32();
                var title = product.GetProperty("title").GetString() ?? "";
                var description = product.GetProperty("description").GetString() ?? "";
                var price = product.GetProperty("price").GetDecimal();
                var category = product.GetProperty("category").GetString() ?? "";
                var thumbnail = product.GetProperty("thumbnail").GetString() ?? "";
                var brand = product.GetProperty("brand").GetString() ?? "";
                var rating = product.GetProperty("rating").GetDouble();
                var stock = product.GetProperty("stock").GetInt32();

                // Escape fields that might contain commas and handle quotes in text
                title = title.Replace("\"", "\"\"");
                description = description.Replace("\"", "\"\"");
                category = category.Replace("\"", "\"\"");
                brand = brand.Replace("\"", "\"\"");

                csvBuilder.AppendLine(
                    $"{id}," +
                    $"\"{title}\"," +
                    $"\"{description}\"," +
                    $"{price}," +
                    $"\"{category}\"," +
                    $"\"{thumbnail}\"," +
                    $"\"{brand}\"," +
                    $"{rating}," +
                    $"{stock}");

                logger.LogInformation($"נוסף מוצר: {title}");
            }
            catch (Exception ex)
            {
                logger.LogWarning($"שגיאה בעיבוד מוצר: {ex.Message}");
                continue;
            }
        }

        // Write to file
        var fileName = "products.csv";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        await File.WriteAllTextAsync(filePath, csvBuilder.ToString());

        logger.LogInformation($"קובץ CSV נוצר בהצלחה בנתיב: {filePath}");
        logger.LogInformation("5 השורות הראשונות בקובץ:");
        var firstLines = (await File.ReadAllLinesAsync(filePath)).Take(6);
        foreach (var line in firstLines)
        {
            logger.LogInformation(line);
        }
    }
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "שגיאה ביצירת קובץ CSV");
}
