using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

var blobServiceClient = new BlobServiceClient(
        new Uri("https://stdotazure001.blob.core.windows.net"),
        new DefaultAzureCredential(),
        options: new BlobClientOptions()
        {
            Retry =
            {
                MaxRetries = 5,
                Mode = Azure.Core.RetryMode.Exponential,
                Delay = TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(10)
            }
        });

string containerName = "quickstartblobs" + Guid.NewGuid().ToString();
var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
await containerClient.CreateIfNotExistsAsync();

string localPath = "data";
Directory.CreateDirectory(localPath);
string fileName = "quickstart" + Guid.NewGuid().ToString() + ".txt";
string localFilePath = Path.Combine(localPath, fileName);

if (!Directory.EnumerateFiles(localPath).Any(f => f.StartsWith("quickstart", StringComparison.OrdinalIgnoreCase)))
{
    await File.WriteAllTextAsync(localFilePath, "Hello, World!");
}

BlobClient blobClient = containerClient.GetBlobClient(fileName);

Console.WriteLine($"Uploading to Blob storage as blob:\n\t {blobClient.Uri}\n");

var uploadOptions = new BlobUploadOptions
{
    Tags = new Dictionary<string, string>
    {
        { "project", "quickstart" },
        { "env", "dev" }
    },

    TransferOptions = new StorageTransferOptions
    {
        // Set the maximum number of parallel transfer workers
        MaximumConcurrency = 2,

        // Set the initial transfer length to 8 MiB
        InitialTransferSize = 8 * 1024 * 1024,

        // Set the maximum length of a transfer to 4 MiB
        MaximumTransferSize = 4 * 1024 * 1024
    }
};

await blobClient.UploadAsync(
    localFilePath,
    options: uploadOptions);

if (await blobClient.ExistsAsync())
{
    Console.WriteLine("Blob exists in the container.");
}
else
{
    Console.WriteLine("Blob does not exist in the container.");
}

Console.WriteLine("Listing blobs...");

// List all blobs in the container
await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
{
    Console.WriteLine("\t" + blobItem.Name);
}


// Download the blob to a local file
// Append the string "DOWNLOADED" before the .txt extension
// so you can compare the files in the data directory
string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

if (!File.Exists(downloadFilePath))
{
    Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

    // Download the blob's contents and save it to a file
    await blobClient.DownloadToAsync(downloadFilePath);

}

// Clean up
Console.Write("Press any key to begin clean up");
Console.ReadLine();

Console.WriteLine("Deleting blob container...");
await containerClient.DeleteAsync();

Console.WriteLine("Deleting the local source and downloaded files...");
File.Delete(localFilePath);
File.Delete(downloadFilePath);

Console.WriteLine("Done");
