using CosmosDbQuickStart.Features.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbQuickStart.Areas.Products.Pages;

public class ProductsCreateModel : PageModel
{
    private readonly ProductService _service;
    [BindProperty]
    public Product Product { get; set; } = new("", "", "", 0, false);

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

        // Ensure id is set before saving
        Product = Product with { id = Guid.NewGuid().ToString() };
        await _service.CreateProductAsync(Product);
        return RedirectToPage("Index");
    }
}
