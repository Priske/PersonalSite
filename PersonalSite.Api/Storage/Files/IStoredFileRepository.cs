using PersonalSite.Api.Domain.Files;

namespace PersonalSite.Api.Storage.Files;

public interface IStoredFileRepository
{
    Task<StoredFile?> GetAsync(int id, CancellationToken cancellationToken);
    void Add(StoredFile file);
    void Remove(StoredFile file);
    Task<bool> IsReferencedByOtherFeaturedContentAsync(
        int id,
        int featuredContentId,
        CancellationToken cancellationToken);
}
