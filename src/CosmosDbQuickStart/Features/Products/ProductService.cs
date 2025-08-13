using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDbQuickStart.Features.Products;

/// <summary>
/// Service for managing products in the Cosmos DB catalog.
/// </summary>
public class ProductService
{
    private readonly ILogger<ProductService> _logger;   
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;
    private const string DatabaseId = "catalog";
    private const string ContainerId = "products";

    public ProductService(CosmosClient cosmosClient, ILogger<ProductService> logger)
    {
        _cosmosClient = cosmosClient;
        _container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
        _logger = logger;
    }

    public async Task<Product?> GetProductAsync(string id, string category = "")
    {
        try
        {
            if (string.IsNullOrEmpty(category))
            {
                category = id; // Use id as partition key if category is not provided
            }

            ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, new PartitionKey(category));

            return response;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
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

    public async Task<IEnumerable<Product>> GetProductsAsync(string category = "")
    {
        var requestOptions = new QueryRequestOptions
        {
            PopulateIndexMetrics = true,
            MaxItemCount = 10, // Adjust as needed
            MaxConcurrency = -1, // Use default concurrency
            MaxBufferedItemCount = 100 // Adjust as needed
        };
        var query = string.IsNullOrEmpty(category)
            ? _container.GetItemQueryIterator<Product>("SELECT * FROM products p", requestOptions: requestOptions)
            : _container.GetItemQueryIterator<Product>(
                new QueryDefinition("SELECT * FROM products p WHERE p.category = @category")
                    .WithParameter("@category", category), 
                requestOptions: requestOptions);

        var results = new List<Product>();
        List<ServerSideCumulativeMetrics> totalMetrics = [];
        while (query.HasMoreResults)
        {
            FeedResponse<Product> response = await query.ReadNextAsync();
            _logger.LogInformation("Index Metrics: {IndexMetrics}", response.IndexMetrics);

            totalMetrics.Add(response.Diagnostics.GetQueryMetrics());

            results.AddRange(response.Resource);
        }

        LogDatabaseMetrics(totalMetrics);

        return results;
    }


    public async Task<Product> CreateProductAsync(Product product)
    {
        ItemResponse<Product> response = await _container.CreateItemAsync(product, new PartitionKey(product.category));

        _logger.LogInformation("Client Elapsed Time: {ClientElapsedTime}", response.Diagnostics.GetClientElapsedTime());

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

    private void LogDatabaseMetrics(List<ServerSideCumulativeMetrics> totalMetrics)
    {
        TimeSpan totalExecutionTime = totalMetrics.Aggregate(TimeSpan.Zero, (currentSum, next) => currentSum + next.CumulativeMetrics.TotalTime);
        double totalRequestCharge = 0;

        var groupedPartitionedMetrics = totalMetrics.SelectMany(m => m.PartitionedMetrics).GroupBy(p => p.PartitionKeyRangeId);
        foreach (var partitionedMetricsItem in groupedPartitionedMetrics)
        {
            foreach (var item in partitionedMetricsItem)
            {
                _logger.LogInformation(
                    "Partition {PartitionKeyRangeId} - Request Charge: {RequestCharge}, Feed Range: {FeedRange}",
                    item.PartitionKeyRangeId,
                    item.RequestCharge,
                    item.FeedRange
                );
                totalRequestCharge += item.RequestCharge;
            }
        }

        _logger.LogInformation("Total request charge for the query: {TotalRequestCharge}", totalRequestCharge);
        _logger.LogInformation("Total time taken for the query: {TotalTime}", totalExecutionTime);
    }

}
