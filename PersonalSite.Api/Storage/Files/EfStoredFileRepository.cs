using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Files;

namespace PersonalSite.Api.Storage.Files;

public sealed class EfStoredFileRepository(
    AppDbContext dbContext)
    : IStoredFileRepository
{
    public Task<StoredFile?> GetAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return dbContext.StoredFiles.SingleOrDefaultAsync(
            file => file.Id == id,
            cancellationToken);
    }

    public void Add(StoredFile file) =>
        dbContext.StoredFiles.Add(file);

    public void Remove(StoredFile file) =>
        dbContext.StoredFiles.Remove(file);

    public Task<bool> IsReferencedByOtherFeaturedContentAsync(
        int id,
        int featuredContentId,
        CancellationToken cancellationToken)
    {
        return dbContext.FeaturedContentFiles.AnyAsync(
            attachment =>
                attachment.StoredFileId == id &&
                attachment.FeaturedContentId != featuredContentId,
            cancellationToken);
    }
}
