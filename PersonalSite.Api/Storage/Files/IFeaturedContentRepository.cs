using PersonalSite.Api.Domain.FeaturedContent;

namespace PersonalSite.Api.Storage.Files;

public interface IFeaturedContentRepository
{
    Task<FeaturedContent?> GetWithFilesAsync(int id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}