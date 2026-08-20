using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.FeaturedContent;

namespace PersonalSite.Api.Storage.Files;

public sealed class EfFeaturedContentRepository(
    AppDbContext dbContext)
    : IFeaturedContentRepository
{
    public async Task<FeaturedContent> AddAsync(
        FeaturedContent content,
        CancellationToken cancellationToken)
    {
        dbContext.FeaturedContents.Add(content);
        await dbContext.SaveChangesAsync(cancellationToken);

        return content;
    }

    public Task<FeaturedContent?> GetWithFilesAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return dbContext.FeaturedContents
            .Include(content => content.Files)
            .ThenInclude(attachment => attachment.File)
            .Include(content => content.Tags)
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
