using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FunctionApp1.Middleware;

public class ExceptionMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        ILogger<ExceptionMiddleware> logger = context.InstanceServices
            .GetRequiredService<ILogger<ExceptionMiddleware>>();

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogProblemException(ex.Message, DateTime.UtcNow);

            var invocationResult = context.GetInvocationResult();
            if (invocationResult != null)
            {
                invocationResult.Value = ex.Message;
            }
        }
    }
}

internal static partial class ExceptionMiddlewareLogger
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Error Message: {Message}, Time of occurrence {Time}")]
    public static partial void LogProblemException(
        this ILogger<ExceptionMiddleware> logger,
        string message, DateTime time);
}
