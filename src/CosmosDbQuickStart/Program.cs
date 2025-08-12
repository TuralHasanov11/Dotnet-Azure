using CosmosDbQuickStart;
using CosmosDbQuickStart.Features.Products;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton(
    s => new CosmosClient(
        accountEndpoint: "https://localhost:8081/", 
        authKeyOrResourceToken: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
// or if using Azure Cosmos DB with managed identity, uncomment the following lines:
//builder.Services.AddSingleton(
//    s => new CosmosClient(
//        accountEndpoint: "<your-cosmos-db-endpoint>", // e.g. https://your-account.documents.azure.com:443/
//        tokenCredential: new DefaultAzureCredential()));

builder.Services.AddScoped<ProductService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

if (app.Environment.IsDevelopment())
{
    await app.SeedDatabase();
}

await app.RunAsync();
