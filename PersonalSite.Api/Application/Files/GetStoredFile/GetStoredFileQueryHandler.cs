using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Storage;
using PersonalSite.Api.Storage.Files;

namespace PersonalSite.Api.Application.Files.GetStoredFile;

public sealed class GetStoredFileQueryHandler(
    AppDbContext dbContext,
    IFileStorage fileStorage) : IHandler
{
    public async Task<StoredFileResponse?> Execute(
        int fileId,
        CancellationToken cancellationToken)
    {
        var storedFile = await dbContext.StoredFiles
            .AsNoTracking()
            .SingleOrDefaultAsync(
                file =>
                    file.Id == fileId &&
                    dbContext.FeaturedContentFiles.Any(attachment =>
                        attachment.StoredFileId == file.Id &&
                        dbContext.FeaturedContents.Any(content =>
                            content.Id == attachment.FeaturedContentId &&
                            content.Source == ContentSource.Official)),
                cancellationToken);

        if (storedFile is null)
        {
            return null;
        }

        var content = await fileStorage.OpenReadAsync(
            storedFile.StorageKey,
            cancellationToken);

        return content is null
            ? null
            : new StoredFileResponse(
                content,
                storedFile.ContentType,
                storedFile.OriginalFileName);
    }
}

public sealed record StoredFileResponse(
    Stream Content,
    string ContentType,
    string FileName);
