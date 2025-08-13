namespace CosmosDbQuickStart.Areas.Products.Pages;

public class ProductEditViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool Sale { get; set; }
}
