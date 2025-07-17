using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp2;

internal class EventHubFunction
{
    private readonly ILogger<EventHubFunction> _logger;

    public EventHubFunction(ILogger<EventHubFunction> logger)
    {
        _logger = logger;
    }


    [Function(nameof(ThrowOnCancellation))]
    public async Task ThrowOnCancellation(
        [EventHubTrigger("sample-workitem-1", Connection = "EventHubConnection")] string[] messages,
        FunctionContext _,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "C# EventHub {functionName} trigger function processing a request.",
            nameof(ThrowOnCancellation));

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(6000); // task delay to simulate message processing
            _logger.LogInformation("Message '{msg}' was processed.", message);
        }
    }

    [Function(nameof(HandleCancellationCleanup))]
    public async Task HandleCancellationCleanup(
        [EventHubTrigger("sample-workitem-2", Connection = "EventHubConnection")] string[] messages,
        FunctionContext _,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "C# EventHub {functionName} trigger function processing a request.",
            nameof(HandleCancellationCleanup));

        foreach (var message in messages)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("A cancellation token was received, taking precautionary actions.");
                // Take precautions like noting how far along you are with processing the batch
                _logger.LogInformation("Precautionary activities complete.");
                break;
            }

            await Task.Delay(6000, cancellationToken); // task delay to simulate message processing
            _logger.LogInformation("Message '{msg}' was processed.", message);
        }
    }
}
