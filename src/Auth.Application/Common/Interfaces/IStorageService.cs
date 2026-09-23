namespace Auth.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> UpLoadFileAsync(string containerName, Stream stream, string contentType, string extension);
    Task DeleteAsync(string containerName, string fileName);
    Task<Stream> DownloadFileAsync(string containerName, string fileName);
}