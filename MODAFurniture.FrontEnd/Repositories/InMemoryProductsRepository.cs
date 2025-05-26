using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MODAFurniture.Core.Entities;
using MODAFurniture.FrontEnd.Interfaces;

namespace MODAFurniture.FrontEnd.Repositories;

public class InMemoryProductsRepository : IProductRepository
{
    // Static collection to hold all products
    private static List<Product>? _products;
    
    // Static property for the data file path
    public static string DataPath { get; set; } = "wwwroot/data/products.json";
    public static string ImagesPath { get; set; } = "wwwroot/images";

    // Static lock object to ensure thread safety during initialization
    private static readonly object _initLock = new();
    
    private ILogger<InMemoryProductsRepository>? _logger;

    /// <summary>
    /// Constructor that ensures products are loaded from the JSON file
    /// </summary>
    public InMemoryProductsRepository(ILogger<InMemoryProductsRepository> logger)
    {
        EnsureDataLoaded();
        _logger = logger;
    }

    /// <summary>
    /// Ensures that the product data is loaded from the JSON file
    /// </summary>
    private static void EnsureDataLoaded()
    {
        if (_products == null)
        {
            lock (_initLock)
            {
                if (_products == null)
                {
                    LoadProductsFromJsonFile().GetAwaiter().GetResult();
                }
            }
        }
    }
    
    /// <summary>
    /// Loads product data from the JSON file
    /// </summary>
    private static async Task LoadProductsFromJsonFile()
    {
        _products = new List<Product>();

        try
        {
            if (File.Exists(DataPath))
            {
                string jsonContent = await File.ReadAllTextAsync(DataPath);
                var productContainer = JsonSerializer.Deserialize<ProductsContainer>(jsonContent);
                foreach (var product in productContainer?.Products ?? new List<Product>())
                {
                    //product.Image = Path.Combine(ImagesPath, product.Image);
                    _products.Add(product);
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load products from {DataPath}", ex);
        }
    }
    
    /// <summary>
    /// Gets all products asynchronously
    /// </summary>
    public Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return Task.FromResult(_products!.AsEnumerable());
    }
    
    /// <summary>
    /// Gets a product by name asynchronously
    /// </summary>
    public Task<Product?> GetProductByNameAsync(string name)
    {
        var product = _products!.FirstOrDefault(p => 
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(product);
    }
    
    /// <summary>
    /// Gets products by category asynchronously
    /// </summary>
    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        var products = _products!.Where(p => 
            p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(products);
    }
    
    /// <summary>
    /// Gets all available categories asynchronously
    /// </summary>
    public Task<IEnumerable<string>> GetCategoriesAsync()
    {
        var categories = _products!
            .Select(p => p.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase);
        return Task.FromResult(categories);
    }
    
    /// <summary>
    /// Searches products by keyword in name or description
    /// </summary>
    public Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Task.FromResult(_products!.AsEnumerable());
            
        searchTerm = searchTerm.ToLower();
        
        var results = _products!.Where(p =>
            p.Name.ToLower().Contains(searchTerm) ||
            p.ShortDescription.ToLower().Contains(searchTerm) ||
            p.LongDescription.ToLower().Contains(searchTerm));
            
        return Task.FromResult(results);
    }
    
    /// <summary>
    /// Adds a new product asynchronously
    /// </summary>
    public Task<Product> AddProductAsync(Product product)
    {
        var existingProduct = _products!.FirstOrDefault(p => 
            p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase));
            
        if (existingProduct != null)
            throw new InvalidOperationException($"A product with the name '{product.Name}' already exists.");
            
        _products.Add(product);
        return Task.FromResult(product);
    }
    
    /// <summary>
    /// Updates an existing product asynchronously
    /// </summary>
    public Task UpdateProductAsync(Product product)
    {
        var index = _products!.FindIndex(p => 
            p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase));
            
        if (index == -1)
            throw new InvalidOperationException($"Product '{product.Name}' not found.");
            
        _products[index] = product;
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Deletes a product asynchronously
    /// </summary>
    public Task DeleteProductAsync(string name)
    {
        var index = _products!.FindIndex(p => 
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            
        if (index == -1)
            throw new InvalidOperationException($"Product '{name}' not found.");
            
        _products.RemoveAt(index);
        return Task.CompletedTask;
    }
}
