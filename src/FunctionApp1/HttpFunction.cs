using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1;

internal class HttpFunction
{
    [Function(nameof(HttpFunction))]
    public static HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData request,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(HttpFunction));
        logger.LogInformation("message logged");

        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString("Welcome to .NET isolated worker !!");

        return response;
    }
}
