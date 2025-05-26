using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MODAFurniture.Core.Entities;
using MODAFurniture.FrontEnd.Interfaces;

namespace MODAFurniture.FrontEnd.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IProductRepository _productRepository;
        public ProductsModel(ILogger<IndexModel> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<string> Categories { get; set; } = new List<string>();
        public string SelectedCategory { get; set; } = string.Empty;


        public async Task OnGetAsync(string category)
        {
            // Store the selected category
            SelectedCategory = category ?? string.Empty;
            // Get all available categories
            Categories = await _productRepository.GetCategoriesAsync();

            // Get products based on selected category or all products if no category is selected
            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                Products = await _productRepository.GetProductsByCategoryAsync(SelectedCategory);
            }
            else
            {
                Products = await _productRepository.GetAllProductsAsync();
            }
        }
    }
}
