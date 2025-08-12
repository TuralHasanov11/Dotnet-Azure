using CosmosDbQuickStart.Features.Products;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CosmosDbQuickStart.Areas.Products.Pages;

public class ProductsIndexModel : PageModel
{
    private readonly ProductService _service;
    public IReadOnlyCollection<Product> Products { get; private set; }

    public ProductsIndexModel(ProductService service)
    {
        _service = service;
    }

    public async Task OnGetAsync()
    {
        Products = (await _service.GetProductsAsync()).ToList().AsReadOnly();
    }
}
