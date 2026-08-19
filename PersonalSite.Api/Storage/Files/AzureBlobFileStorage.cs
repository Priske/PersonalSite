using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace PersonalSite.Api.Storage.Files;

public sealed class AzureBlobFileStorage(
    IConfiguration configuration) : IFileStorage
{
    private BlobContainerClient GetContainerClient()
    {
        var accountName =
            configuration["AzureStorage:AccountName"]
            ?? throw new InvalidOperationException(
                "Azure storage account name is missing.");

        var containerName =
            configuration["AzureStorage:ContainerName"]
            ?? throw new InvalidOperationException(
                "Azure storage container name is missing.");

        var serviceClient = new BlobServiceClient(
            new Uri($"https://{accountName}.blob.core.windows.net"),
            new DefaultAzureCredential());

        return serviceClient.GetBlobContainerClient(containerName);
    }

    public async Task UploadAsync(
     string fileName,
     Stream content,
     string contentType,
     bool overwrite,
     CancellationToken cancellationToken)
    {
        var blobClient =
            GetContainerClient().GetBlobClient(fileName);

        await blobClient.UploadAsync(
            content,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                },

                Conditions = overwrite
                    ? null
                    : new BlobRequestConditions
                    {
                        IfNoneMatch = ETag.All
                    }
            },
            cancellationToken);
    }
    public async Task<Stream?> OpenReadAsync(
    string fileName,
    CancellationToken cancellationToken)
    {
        var blobClient =
            GetContainerClient().GetBlobClient(fileName);

        try
        {
            var response =
                await blobClient.DownloadStreamingAsync(
                    cancellationToken: cancellationToken);

            return response.Value.Content;
        }
        catch (RequestFailedException exception)
            when (exception.Status == StatusCodes.Status404NotFound)
        {
            return null;
        }
    }
    public async Task DeleteAsync(
    string fileName,
    CancellationToken cancellationToken)
    {
        var blobClient =
            GetContainerClient().GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync(
            cancellationToken: cancellationToken);
    }
}