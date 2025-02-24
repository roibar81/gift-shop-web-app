using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApp.Api.Data;
using WebApp.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace CsvGenerator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            // Add logging
            services.AddLogging(configure => configure.AddConsole());

            // Add DbContext
            services.AddDbContext<GiftShopContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=GiftShopDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

            // Add ProductService
            services.AddScoped<ProductService>();

            var serviceProvider = services.BuildServiceProvider();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var productService = scope.ServiceProvider.GetRequiredService<ProductService>();

                    // Get CSV content
                    var csvContent = await productService.ExportProductsToCsvAsync();

                    // Define file name and path
                    var fileName = "products.csv";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                    // Write CSV content to file
                    await File.WriteAllTextAsync(filePath, csvContent);

                    Console.WriteLine($"קובץ CSV נוצר בהצלחה בנתיב: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"שגיאה ביצירת קובץ CSV: {ex.Message}");
            }
        }
    }
} 