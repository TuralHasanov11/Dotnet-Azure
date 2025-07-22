using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using FunctionApp1.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services
    .AddAzureClients(clientBuilder =>
    {
        TokenCredential credential;

        if (builder.Environment.IsProduction() || builder.Environment.IsStaging())
        {
            // Managed identity token credential discovered when running in Azure environments
            credential = new ManagedIdentityCredential();
        }
        else
        {
            // Running locally on dev machine - DO NOT use in production or outside of local dev
            credential = new DefaultAzureCredential();
        }

        clientBuilder.AddBlobServiceClient(builder.Configuration.GetSection("AzureWebJobsStorage"))
            .WithName("copierOutputBlob");

        clientBuilder.UseCredential(credential);
    });

builder.Services.AddOpenTelemetry()
    .UseAzureMonitorExporter()
    .UseFunctionsWorkerDefaults();

builder.Services.AddHttpClient();

builder.Services.AddMvc();

builder.UseMiddleware<ExceptionMiddleware>();
//builder.UseWhen<ExceptionMiddleware>((context) =>
//{
//    // We want to use this middleware only for http trigger invocations.
//    return context.FunctionDefinition.InputBindings.Values
//        .First(a => a.Type.EndsWith("Trigger", StringComparison.InvariantCulture))
//        .Type == "httpTrigger";
//});

builder.Services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
{
    jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    jsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

await builder.Build().RunAsync();
