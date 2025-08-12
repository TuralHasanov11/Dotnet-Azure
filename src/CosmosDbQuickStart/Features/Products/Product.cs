using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CosmosDbQuickStart.Features.Products;

// C# record type for items in the container
public record Product(
    string id,
    string category,
    string name,
    int quantity,
    bool sale
);
