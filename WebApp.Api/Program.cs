using Microsoft.EntityFrameworkCore;
using WebApp.Api.Data;
using WebApp.Api.Services;
using System.Globalization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure globalization
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") };
    options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-US") };
});

// Add configuration sources
builder.Configuration.AddEnvironmentVariables(prefix: "OPENAI_");

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp",
        builder => builder
            .WithOrigins("https://localhost:7000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add HttpClient
builder.Services.AddHttpClient();

// Configure JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Add DbContext
builder.Services.AddDbContext<GiftShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register our services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ChatService>();

var app = builder.Build();

// Configure request localization
app.UseRequestLocalization();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GiftShopContext>();
        context.Database.Migrate();
        
        var productService = services.GetRequiredService<ProductService>();
        await productService.SeedInitialDataAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowWebApp");

app.UseAuthorization();

app.MapControllers();

// Configure the port - moving this before app.Run()
app.Urls.Add("https://localhost:7001");

app.Run();
