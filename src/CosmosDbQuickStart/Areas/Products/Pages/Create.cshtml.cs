using CosmosDbQuickStart.Features.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbQuickStart.Areas.Products.Pages;

public class ProductsCreateModel : PageModel
{
    private readonly ProductService _service;
    [BindProperty]
    public ProductCreateViewModel Product { get; set; } = new();

    public ProductsCreateModel(ProductService service)
    {
        _service = service;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var newProduct = new Product(
            id: Guid.NewGuid().ToString(),
            category: Product.Category,
            name: Product.Name,
            quantity: Product.Quantity,
            sale: Product.Sale
        );
        await _service.CreateProductAsync(newProduct);
        return RedirectToPage("Index");
    }
}
