namespace Auth.Infrastructure.Services;

public class AzureBlobStorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobStorageService()
    {
        var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2024_11_04);
        _blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true", options);
    }

    public async Task<string> UpLoadFileAsync(string containerName, Stream stream, string contentType, string extension)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync();

        var fileName = $"{Guid.NewGuid().ToString()} {extension}";
        var blobClient = containerClient.GetBlobClient(fileName);
        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        };

        await blobClient.UploadAsync(stream, uploadOptions);

        return blobClient.Uri.AbsoluteUri;
    }

    public async Task DeleteAsync(string containerName, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        var response = await blobClient.DownloadAsync();

        return response.Value.Content;
    }
}