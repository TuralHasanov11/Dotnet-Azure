using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace FunctionApp1.Middleware;

internal class StampHttpHeaderMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var requestData = await context.GetHttpRequestDataAsync();

        string correlationId;

        if (requestData!.Headers.TryGetValues("x-correlation-id", out var values))
        {
            correlationId = values.First();
        }
        else
        {
            correlationId = Guid.NewGuid().ToString();
        }

        await next(context);

        context.GetHttpResponseData()?.Headers.Add("x-correlation-id", correlationId);
    }
}
