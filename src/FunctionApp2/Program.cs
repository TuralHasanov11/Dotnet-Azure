using System.Text.Json;
using System.Text.Json.Serialization;
using FunctionApp2.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerApplication =>
    {
        workerApplication.UseMiddleware<ExceptionMiddleware>();
        //workerApplication.UseWhen<ExceptionMiddleware>((context) =>
        //{
        //    // We want to use this middleware only for http trigger invocations.
        //    return context.FunctionDefinition.InputBindings.Values
        //        .First(a => a.Type.EndsWith("Trigger", StringComparison.InvariantCulture))
        //        .Type == "httpTrigger";
        //});

        workerApplication.Services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
        {
            jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            jsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        });
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(hostContext.Configuration.GetSection("MyStorageConnection"))
                .WithName("copierOutputBlob");
        });

    })
    .Build();

await host.RunAsync();
