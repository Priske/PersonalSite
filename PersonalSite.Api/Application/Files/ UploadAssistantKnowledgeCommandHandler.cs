using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Files;
using PersonalSite.Api.Storage.Files;

namespace PersonalSite.Api.Application.Files;

public sealed class UploadAssistantKnowledgeCommandHandler(
    IFileStorage fileStorage) : IHandler
{
    private static readonly FileRules Rules = new(
        AllowedExtensions:
        [
            ".md"
        ],
        AllowedContentTypes:
        [
            "text/markdown",
            "text/plain"
        ],
        MaxFileSize: 256 * 1024);

    public async Task ExecuteAsync(
    Actor actor,
    Stream content,
    string fileName,
    string contentType,
    long fileSize,
    CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new UnauthorizedAccessException(
                "Only administrators can upload the Assistants KnowledgeFiles.");
        }

        FileValidator.Validate(
            fileName,
            contentType,
            fileSize,
            Rules);

        var safeFileName = Path
            .GetFileName(fileName)
            .ToLowerInvariant();

        var storageKey =
            $"assistant/knowledge/{safeFileName}";

        await fileStorage.UploadAsync(
            storageKey,
            content,
            "text/markdown",
            overwrite: true,
            cancellationToken);
    }
}