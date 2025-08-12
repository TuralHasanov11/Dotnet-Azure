using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDbQuickStart.Features.Products;

public class ProductService
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;
    private const string DatabaseId = "catalog";
    private const string ContainerId = "products";

    public ProductService(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
    }

    public async Task<Product?> GetProductAsync(string id, string category = "")
    {
        try
        {
            if (string.IsNullOrEmpty(category))
            {
                category = id; // Use id as partition key if category is not provided
            }

            Product response = await _container.ReadItemAsync<Product>(id, new PartitionKey(category));

            return response;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(string category = "")
    {
        var query = string.IsNullOrEmpty(category)
            ? _container.GetItemQueryIterator<Product>("SELECT * FROM products p")
            : _container.GetItemQueryIterator<Product>(
                new QueryDefinition("SELECT * FROM products p WHERE p.category = @category")
                    .WithParameter("@category", category));

        var results = new List<Product>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }
        return results;
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        var response = await _container.CreateItemAsync(product, new PartitionKey(product.category));
        return response.Resource;
    }

    public async Task<Product?> UpdateProductAsync(Product product)
    {
        var response = await _container.UpsertItemAsync(product, new PartitionKey(product.category));
        return response.Resource;
    }

    public async Task<bool> DeleteProductAsync(Product product)
    {
        try
        {
            await _container.DeleteItemAsync<Product>(product.id, new PartitionKey(product.category));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<Product?> GetProductAsync(string id)
    {
        IQueryable<Product> queryable = _container.GetItemLinqQueryable<Product>().Where(p => p.id == id);

        using FeedIterator<Product> iterator = queryable.ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            var product = response.FirstOrDefault();
            if (product != null)
            {
                return product;
            }
        }
        return null;
    }
}
