using WebApp.Web.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Configure Blazor
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});

// Register services
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ProductService>();

// Configure the port
builder.WebHost.UseUrls("https://localhost:7000");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configure endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
