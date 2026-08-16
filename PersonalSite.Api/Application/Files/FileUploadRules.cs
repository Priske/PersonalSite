namespace PersonalSite.Api.Application.Files;

public sealed record FileUploadRules(
    IReadOnlyCollection<string> AllowedExtensions,
    IReadOnlyCollection<string> AllowedContentTypes,
    long MaxFileSize);