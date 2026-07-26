using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Storage.Tags;

public sealed class EfTagRepository(AppDbContext dbContext) : ITagRepository
{
    public async Task<Tag> AddAsync(Tag tag)
    {
        dbContext.Tags.Add(tag);

        await dbContext.SaveChangesAsync();

        return tag;
    }

    public async Task<bool> UpdateAsync(Tag tag)
    {
        var existingTag = await dbContext.Tags
            .SingleOrDefaultAsync(current => current.Id == tag.Id);

        if (existingTag is null)
        {
            return false;
        }

        existingTag.Name = tag.Name;

        await dbContext.SaveChangesAsync();

        return true;
    }
    public async Task<IReadOnlyList<Tag>> GetByIdsAsync(
    IReadOnlyCollection<int> ids)
    {
        return await dbContext.Tags
            .Where(tag => ids.Contains(tag.Id))
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tag = await dbContext.Tags
            .SingleOrDefaultAsync(current => current.Id == id);

        if (tag is null)
        {
            return false;
        }

        dbContext.Tags.Remove(tag);

        await dbContext.SaveChangesAsync();

        return true;
    }

    public Task<bool> TagExistsAsync(TagName name, int? tagIdToIgnore = null)
        => dbContext.Tags.AnyAsync(current =>
            current.Name == name &&
            (!tagIdToIgnore.HasValue || current.Id != tagIdToIgnore.Value));

    public Task<Tag?> GetByIdAsync(int id)
        => dbContext.Tags.SingleOrDefaultAsync(current => current.Id == id);
}