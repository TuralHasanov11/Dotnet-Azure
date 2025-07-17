using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp2;

internal class QueueFunction
{
    private readonly ILogger<QueueFunction> _logger;

    public QueueFunction(ILogger<QueueFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(QueueFunction))]
    [QueueOutput("output-queue")]
    public async Task<string[]> Run(
        [QueueTrigger("input-queue")] Album albumQueueItem,
        FunctionContext _,
        CancellationToken cancellationToken)
    {

        if (albumQueueItem == null)
        {
            throw new ArgumentNullException(nameof(albumQueueItem), "Queue item cannot be null");
        }

        await Task.Delay(3000, cancellationToken);

        _logger.LogInformation(
            "Processing album: {Name} by {Artist}",
            albumQueueItem.Name,
            albumQueueItem.Artist);

        return [
            $"Processed album: {albumQueueItem.Name} by {albumQueueItem.Artist}",
        ];
    }
}
