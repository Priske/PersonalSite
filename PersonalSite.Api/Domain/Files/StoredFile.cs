namespace PersonalSite.Api.Domain.Files;

public sealed class StoredFile
{
    private StoredFile() { }

    public int Id { get; private set; }

    public string StorageKey { get; private set; } = null!;
    public string OriginalFileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeInBytes { get; private set; }

    public static StoredFile Create(
        string storageKey,
        string originalFileName,
        string contentType,
        long sizeInBytes)
    {
        return new StoredFile
        {
            StorageKey = storageKey,
            OriginalFileName = Path.GetFileName(originalFileName),
            ContentType = contentType,
            SizeInBytes = sizeInBytes
        };
    }

}