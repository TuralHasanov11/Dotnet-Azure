using CosmosDbQuickStart.Features.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbQuickStart.Areas.Products.Pages;

public class ProductsDeleteModel : PageModel
{
    private readonly ProductService _service;
    [BindProperty]
    public Product Product { get; set; } = new("", "", "", 0, false);

    public ProductsDeleteModel(ProductService service)
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

        Product = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var product = await _service.GetProductAsync(id);
        if (product == null)
        {
            return RedirectToPage("Index");
        }

        Product = product;

        await _service.DeleteProductAsync(Product);
        return RedirectToPage("Index");
    }
}
