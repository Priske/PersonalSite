using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Storage.Tags;

public interface ITagRepository
{
    Task<Tag> AddAsync(Tag tag);

    Task<Tag?> GetByIdAsync(int id);

    Task<IReadOnlyList<Tag>> GetByIdsAsync(IReadOnlyCollection<int> ids);

    Task<bool> UpdateAsync(Tag tag);

    Task<bool> DeleteAsync(int id);

    Task<bool> TagExistsAsync(
        TagName name,
        int? excludeId = null);
}