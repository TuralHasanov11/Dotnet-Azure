using CosmosDbQuickStart.Features.Products;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbQuickStart.Areas.Products.Pages;

public class ProductsDetailsModel : PageModel
{
    private readonly ProductService _service;
    public Product Product { get; private set; } = new("", "", "", 0, false);

    public ProductsDetailsModel(ProductService service)
    {
        _service = service;
    }

    public async Task OnGetAsync(string id)
    {
        Product = await _service.GetProductAsync(id) ?? new("", "", "", 0, false);
    }
}
