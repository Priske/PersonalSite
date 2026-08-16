namespace PersonalSite.Api.Storage.Files;

public interface IFileStorage
{
    Task UploadAsync(
        string fileName,
        Stream content,
        string contentType,
        bool overwrite,
        CancellationToken cancellationToken);

    Task<Stream?> OpenReadAsync(
        string fileName,
        CancellationToken cancellationToken);
}