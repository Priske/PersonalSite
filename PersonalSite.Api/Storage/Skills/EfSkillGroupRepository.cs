
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Storage.Skills;

public class EfSkillGroupRepository(AppDbContext dbContext) : ISkillGroupRepository
{
    public async Task<SkillGroup> AddAsync(SkillGroup skillgroup)
    {
        dbContext.SkillGroups.Add(skillgroup);
        await dbContext.SaveChangesAsync();
        return skillgroup;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var skillgroup = await dbContext.SkillGroups.FindAsync(id);
        if (skillgroup is null) return false;
        dbContext.SkillGroups.Remove(skillgroup);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<SkillGroup?> GetByIdAsync(int id)
    {
        return await dbContext.SkillGroups
           .AsNoTracking()
           .SingleOrDefaultAsync(skillgroup => skillgroup.Id == id);
    }

    public async Task<bool> SkillExistsAsync(SkillGroup skillgroup)
    {
        var exists = await dbContext.SkillGroups.FindAsync(skillgroup.Id);
        if (exists is null) return false;
        return true;
    }

    public async Task<bool> UpdateAsync(SkillGroup skillgroup)
    {
        var existingSkillGroup = await dbContext.SkillGroups.FindAsync(skillgroup.Id);

        if (existingSkillGroup is null)
        {
            return false;
        }

        existingSkillGroup.Name = skillgroup.Name;
        existingSkillGroup.DisplayOrder = skillgroup.DisplayOrder;

        await dbContext.SaveChangesAsync();

        return true;
    }
    public async Task<bool> UpdateOrderAsync(
    IReadOnlyList<int> skillGroupIds,
    CancellationToken cancellationToken)
    {
        if (skillGroupIds.Count == 0)
        {
            return false;
        }

        if (skillGroupIds.Distinct().Count() != skillGroupIds.Count)
        {
            throw new DomainException(
                "A skill group cannot appear more than once.");
        }

        var groups = await dbContext.SkillGroups
            .Where(group => skillGroupIds.Contains(group.Id))
            .ToListAsync(cancellationToken);

        if (groups.Count != skillGroupIds.Count)
        {
            throw new DomainException(
                "One or more skill groups do not exist.");
        }

        var groupsById = groups.ToDictionary(group => group.Id);

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        for (var index = 0; index < skillGroupIds.Count; index++)
        {
            groupsById[skillGroupIds[index]].DisplayOrder = 10_000 + index;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        for (var index = 0; index < skillGroupIds.Count; index++)
        {
            groupsById[skillGroupIds[index]].DisplayOrder = index + 1;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}