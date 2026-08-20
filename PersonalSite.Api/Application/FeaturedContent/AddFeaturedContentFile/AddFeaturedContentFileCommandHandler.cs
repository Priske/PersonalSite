using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.FeaturedContent;
using PersonalSite.Api.Domain.Files;
using PersonalSite.Api.Storage.Files;

namespace PersonalSite.Api.Application.FeaturedContent.AddFeaturedContentFile;

public sealed class AddFeaturedContentFileCommandHandler(
    IFileStorage fileStorage,
    IStoredFileRepository storedFileRepository,
    IFeaturedContentRepository featuredContentRepository)
    : IHandler
{
    private static readonly FileRules VideoRules = new(
        [".mp4", ".webm"],
        ["video/mp4", "video/webm"],
        100 * 1024 * 1024);

    private static readonly FileRules ImageRules = new(
        [".jpg", ".jpeg", ".png", ".webp"],
        ["image/jpeg", "image/png", "image/webp"],
        10 * 1024 * 1024);

    private static readonly FileRules DocumentRules = new(
        [".pdf"],
        ["application/pdf"],
        10 * 1024 * 1024);

    public async Task<AddFeaturedContentFileResponse?> Execute(
        Actor actor,
        int featuredContentId,
        Stream content,
        string originalFileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new ForbiddenOperationException(
                "Only administrators can modify featured content.");
        }

        var featuredContent =
            await featuredContentRepository.GetWithFilesAsync(
                featuredContentId,
                cancellationToken);

        if (featuredContent is null)
        {
            return null;
        }

        var rules = ResolveRules(contentType);

        FileValidator.Validate(
            originalFileName,
            contentType,
            sizeInBytes,
            rules);

        var extension = Path.GetExtension(originalFileName)
            .ToLowerInvariant();

        var storageKey =
            $"featured-content/{featuredContentId}/" +
            $"{Guid.NewGuid():N}{extension}";

        var storedFile = StoredFile.Create(
            storageKey,
            originalFileName,
            contentType,
            sizeInBytes);

        await fileStorage.UploadAsync(
            storageKey,
            content,
            contentType,
            overwrite: false,
            cancellationToken);

        storedFileRepository.Add(storedFile);

        featuredContent.Files.Add(
            new FeaturedContentFile(storedFile));

        try
        {
            await featuredContentRepository.SaveChangesAsync(
                cancellationToken);
        }
        catch
        {
            await fileStorage.DeleteAsync(
                storageKey,
                CancellationToken.None);

            throw;
        }

        return new AddFeaturedContentFileResponse(
            storedFile.Id,
            storedFile.OriginalFileName,
            storedFile.ContentType,
            storedFile.SizeInBytes);
    }

    private static FileRules ResolveRules(string contentType)
    {
        if (VideoRules.AllowedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            return VideoRules;
        }

        if (ImageRules.AllowedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            return ImageRules;
        }

        if (DocumentRules.AllowedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            return DocumentRules;
        }

        throw new ArgumentException(
            $"Content type '{contentType}' is not supported.");
    }
}
