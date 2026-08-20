using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Storage.Files;

namespace PersonalSite.Api.Application.FeaturedContent.RemoveFeaturedContentFile;

public sealed class RemoveFeaturedContentFileCommandHandler(
    IFileStorage fileStorage,
    IStoredFileRepository storedFileRepository,
    IFeaturedContentRepository featuredContentRepository)
    : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int featuredContentId,
        int storedFileId,
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
            return false;
        }

        var attachment = featuredContent.Files.FirstOrDefault(
            file => file.StoredFileId == storedFileId);

        if (attachment is null)
        {
            return false;
        }

        var isReferencedElsewhere =
            await storedFileRepository
                .IsReferencedByOtherFeaturedContentAsync(
                    storedFileId,
                    featuredContentId,
                    cancellationToken);

        featuredContent.Files.Remove(attachment);

        if (!isReferencedElsewhere)
        {
            await fileStorage.DeleteAsync(
                attachment.File.StorageKey,
                cancellationToken);

            storedFileRepository.Remove(attachment.File);
        }

        await featuredContentRepository.SaveChangesAsync(
            isReferencedElsewhere
                ? cancellationToken
                : CancellationToken.None);

        return true;
    }
}
