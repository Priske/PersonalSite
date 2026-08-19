namespace PersonalSite.Api.Domain.Files;

public sealed class StoredFile
{
    private StoredFile()
    {
    }

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
        ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(originalFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        if (sizeInBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sizeInBytes),
                "File size must be greater than zero.");
        }

        return new StoredFile
        {
            StorageKey = storageKey,
            OriginalFileName = Path.GetFileName(originalFileName),
            ContentType = contentType,
            SizeInBytes = sizeInBytes
        };
    }
}