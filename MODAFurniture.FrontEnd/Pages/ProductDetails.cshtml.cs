using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MODAFurniture.Core.Entities;
using MODAFurniture.FrontEnd.Interfaces;

namespace MODAFurniture.FrontEnd.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly IProductRepository _productRepository;

        public ProductDetailsModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return NotFound();
            }

            Product = await _productRepository.GetProductByNameAsync(name);

            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
