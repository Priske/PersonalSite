using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Storage.Tags;

public sealed class EfTagRepository(AppDbContext dbContext) : ITagRepository
{
    public async Task<Tag> AddAsync(Tag tag, CancellationToken cancellationToken)
    {
        dbContext.Tags.Add(tag);

        await dbContext.SaveChangesAsync(cancellationToken);

        return tag;
    }

    public async Task<bool> UpdateAsync(
    Tag tag,
    CancellationToken cancellationToken)
    {
        var existingTag = await dbContext.Tags
            .SingleOrDefaultAsync(
                current => current.Id == tag.Id,
                cancellationToken);

        if (existingTag is null)
        {
            return false;
        }

        existingTag.Name = tag.Name;
        existingTag.Edited = tag.Edited;

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return true;
    }
    public async Task<IReadOnlyList<Tag>> GetByIdsAsync(
    IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        return await dbContext.Tags
            .Where(tag => ids.Contains(tag.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(
     Tag tag,
     CancellationToken cancellationToken)
    {
        dbContext.Tags.Remove(tag);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public Task<bool> TagExistsAsync(TagName name, int? tagIdToIgnore = null)
        => dbContext.Tags.AnyAsync(current =>
            current.Name == name &&
            (!tagIdToIgnore.HasValue || current.Id != tagIdToIgnore.Value));

    public Task<Tag?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => dbContext.Tags.SingleOrDefaultAsync(current => current.Id == id, cancellationToken);
}