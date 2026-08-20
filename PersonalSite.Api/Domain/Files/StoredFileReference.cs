namespace PersonalSite.Api.Domain.Files;

public sealed record StoredFileReference(
    string StorageKey,
    string ContentType,
    string OriginalFileName,
    long Size);