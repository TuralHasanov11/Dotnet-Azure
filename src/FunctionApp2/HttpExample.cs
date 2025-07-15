using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp2;

public static class HttpExample
{
    [Function("HttpExample")]
    public static MultiResponse Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData request,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("HttpExample");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        string message = "Welcome to Azure Functions!";

        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(message);

        // Return a response to both HTTP trigger and storage output binding.
        return new MultiResponse()
        {
            // Write a single message.
            Messages = [message],
            HttpResponse = response
        };
    }
}
