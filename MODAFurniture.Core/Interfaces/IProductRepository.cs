using MODAFurniture.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MODAFurniture.FrontEnd.Interfaces;

public interface IProductRepository
{
    // Get all products
    Task<IEnumerable<Product>> GetAllProductsAsync();

    // Get a specific product by name
    Task<Product?> GetProductByNameAsync(string name);

    // Get products by category
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);

    // Get all available categories
    Task<IEnumerable<string>> GetCategoriesAsync();

    // Search products by keyword in name or description
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);

    // Add a new product
    Task<Product> AddProductAsync(Product product);

    // Update an existing product
    Task UpdateProductAsync(Product product);

    // Delete a product
    Task DeleteProductAsync(string name);

}
