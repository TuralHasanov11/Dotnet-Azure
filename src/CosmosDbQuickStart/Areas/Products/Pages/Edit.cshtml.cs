using CosmosDbQuickStart.Features.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbQuickStart.Areas.Products.Pages;

public class ProductsEditModel : PageModel
{
    private readonly ProductService _service;
    [BindProperty]
    public ProductEditViewModel Product { get; set; } = new();

    public ProductsEditModel(ProductService service)
    {
        _service = service;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var product = await _service.GetProductAsync(id);
        if (product == null)
        {
            return RedirectToPage("Index");
        }

        Product = new ProductEditViewModel
        {
            Id = product.id,
            Name = product.name,
            Category = product.category,
            Quantity = product.quantity,
            Sale = product.sale
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var updatedProduct = new Product(
            id: Product.Id,
            category: Product.Category,
            name: Product.Name,
            quantity: Product.Quantity,
            sale: Product.Sale
        );
        await _service.UpdateProductAsync(updatedProduct);
        return RedirectToPage("Index");
    }
}
