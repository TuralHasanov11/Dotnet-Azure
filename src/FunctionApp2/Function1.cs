using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp2;

public class Function1RequestData
{
    public string Name { get; set; }
}


public static class Function1
{
    [Function("Function1")]
    public static async Task<MultiResponse> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData request,
        FunctionContext executionContext)
    {
        var log = executionContext.GetLogger("Function1");
        log.LogInformation("HTTP trigger function processed a request.");

        string? name = request.Query["name"];

        using var reader = new StreamReader(request.Body);
        string requestBody = await reader.ReadToEndAsync();

        if (!string.IsNullOrEmpty(requestBody))
        {
            Function1RequestData? data = JsonSerializer.Deserialize<Function1RequestData>(requestBody);
            name ??= data?.Name;
        }

        string responseMessage = string.IsNullOrEmpty(name)
            ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            : $"Hello, {name}. This HTTP triggered function executed successfully.";

        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        await response.WriteStringAsync(responseMessage);

        return new MultiResponse
        {
            Messages = [responseMessage],
            HttpResponse = response
        };
    }
}
