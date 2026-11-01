using System.Text;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Assistant;
using PersonalSite.Api.Domain.Files;
using PersonalSite.Api.Storage.Assistant;
using PersonalSite.Api.Storage.Files;
using PersonalSite.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace PersonalSite.Api.Application.Files;

public sealed class UploadAssistantKnowledgeCommandHandler(
    IFileStorage fileStorage,
    IStoredFileRepository storedFileRepository,
    IAssistantKnowledgeRepository knowledgeRepository,
    ILogger<UploadAssistantKnowledgeCommandHandler> logger,
    AppDbContext dbContext)
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
        CancellationToken cancellationToken,
        bool replaceExisting = false)
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

        // All writes serialize, including legacy append uploads. New blobs use a
        // unique key so readers never see partially overwritten active content.
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        if (dbContext.Database.IsNpgsql())
            await dbContext.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(34034)", cancellationToken);
        var storageKey = replaceExisting
            ? $"assistant/knowledge/{Guid.NewGuid():N}/{safeFileName}"
            : $"assistant/knowledge/{safeFileName}";

        var knowledge =
            await knowledgeRepository.GetWithFilesAsync(
                cancellationToken);

        if (knowledge is null)
        {
            knowledge = AssistantKnowledge.Create();
            knowledgeRepository.Add(knowledge);
        }

        var matchingAttachments = knowledge.Files.Where(
            attachment =>
                string.Equals(
                    attachment.File.OriginalFileName,
                    safeFileName,
                    StringComparison.OrdinalIgnoreCase)).ToArray();

        if (matchingAttachments.Length > 0 && !replaceExisting)
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

        // Only the uploaded filename may be replaced. Never clear the collection
        // or detach files with different names, including manually uploaded ones.
        var previous = matchingAttachments.Select(attachment => attachment.File).ToArray();
        foreach (var attachment in matchingAttachments) knowledge.Files.Remove(attachment);
        foreach (var previousFile in previous)
        {
            storedFileRepository.Remove(previousFile);
        }
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

        // A failed/ambiguous commit must not delete the possibly active new blob.
        await transaction.CommitAsync(cancellationToken);

        // Detach only same-name knowledge atomically before deleting its old blobs. Cleanup
        // failure must never roll back the active set or delete the new blob.
        var cleanupFailed = false;
        foreach (var previousFile in previous)
        {
            try { await fileStorage.DeleteAsync(previousFile.StorageKey, CancellationToken.None); }
            catch
            {
                cleanupFailed = true;
                logger.LogWarning("Replaced knowledge file blob cleanup failed for {StorageKey}.", previousFile.StorageKey);
            }
        }
        if (cleanupFailed)
            throw new InvalidOperationException("The same-name knowledge file was replaced, but its old blob cleanup failed. Do not retry automatically.");
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
