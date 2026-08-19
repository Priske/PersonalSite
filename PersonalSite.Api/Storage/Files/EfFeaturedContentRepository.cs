using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.FeaturedContent;

namespace PersonalSite.Api.Storage.Files;

public sealed class EfFeaturedContentRepository(
    AppDbContext dbContext)
    : IFeaturedContentRepository
{
    public Task<FeaturedContent?> GetWithFilesAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return dbContext.FeaturedContents
            .Include(content => content.Files)
            .ThenInclude(attachment => attachment.File)
            .SingleOrDefaultAsync(
                content => content.Id == id,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
    CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
