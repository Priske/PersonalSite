using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Storage.Tags;

public interface ITagRepository
{
    Task<Tag> AddAsync(Tag tag);

    Task<bool> UpdateAsync(Tag tag);

    Task<bool> DeleteAsync(int id);

    Task<bool> TagExistsAsync(TagName name, int? tagIdToIgnore = null);

    Task<Tag?> GetByIdAsync(int id);
}