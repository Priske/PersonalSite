using PersonalSite.Api.Domain.FeaturedContent;

namespace PersonalSite.Api.Storage.Files;

public interface IFeaturedContentRepository
{
    Task<FeaturedContent> AddAsync(
        FeaturedContent content,
        CancellationToken cancellationToken);

    Task<FeaturedContent?> GetWithFilesAsync(int id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
