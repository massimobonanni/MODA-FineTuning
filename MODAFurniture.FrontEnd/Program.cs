using Microsoft.Extensions.AI;
using MODAFurniture.FrontEnd.Interfaces;
using MODAFurniture.FrontEnd.Repositories;
using MODAFurniture.FrontEnd.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();

builder.Services.AddSingleton<DeploymentsManager>(s =>
{
    var manager = new DeploymentsManager(s.GetRequiredService<IConfiguration>());
    manager.CreateClients();
    return manager;
});
builder.Services.AddTransient<IProductRepository, InMemoryProductsRepository>(s =>
{
    var logger = s.GetRequiredService<ILogger<InMemoryProductsRepository>>();
    InMemoryProductsRepository.DataPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "data", "products.json");
    InMemoryProductsRepository.ImagesPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "images");
    return new InMemoryProductsRepository(logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
