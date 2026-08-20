namespace PersonalSite.Api.Domain.Files;

public sealed record FileRules(
    IReadOnlyCollection<string> AllowedExtensions,
    IReadOnlyCollection<string> AllowedContentTypes,
    long MaxFileSize);