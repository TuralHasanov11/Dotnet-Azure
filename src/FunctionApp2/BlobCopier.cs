using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

namespace FunctionApp2;

internal class BlobCopier
{
    private readonly ILogger<BlobCopier> _logger;
    private readonly BlobContainerClient _copyContainerClient;

    public BlobCopier(
        ILogger<BlobCopier> logger,
        IAzureClientFactory<BlobServiceClient> blobClientFactory)
    {
        _logger = logger;
        _copyContainerClient = blobClientFactory.CreateClient("copierOutputBlob")
            .GetBlobContainerClient("samples-workitems-copy");
        _copyContainerClient.CreateIfNotExists();
    }

    [Function("BlobCopier")]
    public async Task Run(
        [BlobTrigger("samples-workitems/{name}", Connection = "MyStorageConnection")] Stream blob, string name)
    {
        await _copyContainerClient.UploadBlobAsync(name, blob);
        _logger.LogInformation($"Blob {name} copied!");
    }
}
