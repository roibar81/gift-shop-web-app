using Microsoft.EntityFrameworkCore;
using WebApp.Api.Models;

namespace WebApp.Api.Data;

public class GiftShopContext : DbContext
{
    public GiftShopContext(DbContextOptions<GiftShopContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add any additional model configurations here
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
    }
} 