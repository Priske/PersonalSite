using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Storage.Tags;

public interface ITagRepository
{
    Task<Tag> AddAsync(Tag tag, CancellationToken cancellationToken);

    Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Tag>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Tag tag, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<bool> TagExistsAsync(
        TagName name,
        int? excludeId = null);
}