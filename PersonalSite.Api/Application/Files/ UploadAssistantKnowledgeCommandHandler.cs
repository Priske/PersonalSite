using System.Text;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Assistant;
using PersonalSite.Api.Domain.Files;
using PersonalSite.Api.Storage.Assistant;
using PersonalSite.Api.Storage.Files;

namespace PersonalSite.Api.Application.Files;

public sealed class UploadAssistantKnowledgeCommandHandler(
    IFileStorage fileStorage,
    IStoredFileRepository storedFileRepository,
    IAssistantKnowledgeRepository knowledgeRepository,
    ILogger<UploadAssistantKnowledgeCommandHandler> logger)
    : IHandler
{
    private static readonly FileRules Rules = new(
        AllowedExtensions:
        [
            ".md"
        ],
        AllowedContentTypes:
        [
            "text/markdown",
            "text/plain",
            "application/octet-stream"
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
                "Only administrators can upload assistant knowledge.");
        }

        FileValidator.Validate(
            fileName,
            contentType,
            fileSize,
            Rules);

        var safeFileName = Path
            .GetFileName(fileName)
            .Trim()
            .ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            throw new ArgumentException(
                "The knowledge filename is invalid.");
        }

        var storageKey =
            $"assistant/knowledge/{safeFileName}";

        var knowledge =
            await knowledgeRepository.GetWithFilesAsync(
                cancellationToken);

        if (knowledge is null)
        {
            knowledge = AssistantKnowledge.Create();
            knowledgeRepository.Add(knowledge);
        }

        var fileAlreadyExists = knowledge.Files.Any(
            attachment =>
                string.Equals(
                    attachment.File.StorageKey,
                    storageKey,
                    StringComparison.OrdinalIgnoreCase));

        if (fileAlreadyExists)
        {
            throw new ArgumentException(
                $"A knowledge file named '{safeFileName}' already exists.");
        }

        await using var bufferedContent = new MemoryStream();

        await content.CopyToAsync(
            bufferedContent,
            cancellationToken);

        ValidateMarkdownContent(
            bufferedContent.ToArray());

        bufferedContent.Position = 0;

        await fileStorage.UploadAsync(
            storageKey,
            bufferedContent,
            "text/markdown",
            overwrite: false,
            cancellationToken);

        var storedFile = StoredFile.Create(
            storageKey,
            safeFileName,
            "text/markdown",
            bufferedContent.Length);

        storedFileRepository.Add(storedFile);

        knowledge.Files.Add(
            new AssistantKnowledgeFile(storedFile));

        try
        {
            await knowledgeRepository.SaveChangesAsync(
                cancellationToken);
        }
        catch
        {
            try
            {
                await fileStorage.DeleteAsync(
                    storageKey,
                    CancellationToken.None);
            }
            catch (Exception cleanupException)
            {
                logger.LogWarning(
                    cleanupException,
                    "Failed to remove assistant knowledge blob {StorageKey} after a database error.",
                    storageKey);
            }

            throw;
        }
    }

    private static void ValidateMarkdownContent(
        byte[] content)
    {
        string markdown;

        try
        {
            markdown = new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true)
                .GetString(content);
        }
        catch (DecoderFallbackException exception)
        {
            throw new ArgumentException(
                "The knowledge file must contain valid UTF-8 text.",
                exception);
        }

        if (string.IsNullOrWhiteSpace(markdown))
        {
            throw new ArgumentException(
                "The knowledge file cannot be empty.");
        }
    }
}