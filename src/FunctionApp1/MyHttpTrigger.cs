using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp1;

public class MyHttpTrigger
{
    private readonly HttpClient _client;
    private readonly ILogger<MyHttpTrigger> _log;

    public MyHttpTrigger(IHttpClientFactory httpClientFactory, ILogger<MyHttpTrigger> log)
    {
        _client = httpClientFactory.CreateClient();
        _log = log;
    }

    [Function("MyHttpTrigger")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest _,
        CancellationToken cancellationToken)
    {
        _log.LogInformation("C# HTTP trigger function processed a request.");
        var response = await _client.GetAsync(new Uri("https://microsoft.com"), cancellationToken);
        return new OkObjectResult(await response.Content.ReadAsStringAsync(cancellationToken));
    }
}
