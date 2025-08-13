using CosmosDbQuickStart.Features.Products;
using Microsoft.Azure.Cosmos;

namespace CosmosDbQuickStart
{
    public static class Extensions
    {
        public static async Task SeedDatabase(this WebApplication app)
        {
            CosmosClient cosmosClient = app.Services.GetRequiredService<CosmosClient>();

            var database = await cosmosClient.CreateDatabaseIfNotExistsAsync("catalog");

            if (database.StatusCode is not System.Net.HttpStatusCode.Created and
                not System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Failed to create or access database: {database.StatusCode}");
                return;
            }

            var container = await database.Database.CreateContainerIfNotExistsAsync(
                new ContainerProperties
                {
                    Id = "products",
                    PartitionKeyPath = "/category"
                });

            var query = new QueryDefinition("SELECT VALUE COUNT(1) FROM products p");
            var iterator = container.Container.GetItemQueryIterator<int>(query);
            int productCount = 0;
            while (iterator.HasMoreResults)
            {
                foreach (var count in await iterator.ReadNextAsync())
                {
                    productCount += count;
                }
            }

            if (productCount == 0)
            {
                var products = new List<Product>
                {
                    new(
                        id: Guid.NewGuid().ToString(),
                        category: "gear-surf-surfboards",
                        name: "Sunnox Surfboard",
                        quantity: 8,
                        sale: true
                    ),
                    new(
                        id: Guid.NewGuid().ToString(),
                        category: "gear-surf-wetsuits",
                        name: "Sunnox Wetsuit",
                        quantity: 5,
                        sale: false
                    ),
                    new(
                        id: Guid.NewGuid().ToString(),
                        category: "gear-surf-accessories",
                        name: "Sunnox Surf Wax",
                        quantity: 20,
                        sale: true
                    ),
                };

                List<Task> tasks = new(products.Count);

                foreach (var product in products)
                {
                    tasks.Add(
                        container.Container.CreateItemAsync(product, new PartitionKey(product.category))
                            .ContinueWith(itemResponse =>
                            {
                                if (!itemResponse.IsCompletedSuccessfully)
                                {
                                    AggregateException? innerExceptions = itemResponse?.Exception?.Flatten();
                                    if (innerExceptions?.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                                    {
                                        Console.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Exception {innerExceptions?.InnerExceptions.FirstOrDefault()}.");
                                    }
                                }
                            }));
                }

                await Task.WhenAll(tasks);

                Console.WriteLine("Database seeded with initial products.");
            }
        }
    }
}
