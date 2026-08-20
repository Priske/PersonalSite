
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Storage.Skills;

public class EfSkillRepository(AppDbContext dbContext) : ISkillRepository
{

    public async Task UpdateOrderAsync(
   int groupId,
   IReadOnlyList<int> skillIds,
   CancellationToken cancellationToken)
    {
        if (skillIds.Count == 0)
        {
            return;
        }

        if (skillIds.Distinct().Count() != skillIds.Count)
        {
            throw new ArgumentException(
                "A skill cannot appear more than once.");
        }

        var skills = await dbContext.Skills
            .Where(skill => skill.SkillGroupId == groupId)
            .ToListAsync(cancellationToken);

        if (skills.Count != skillIds.Count)
        {
            throw new ArgumentException(
                "The supplied skill list does not contain every skill in the group.");
        }

        var skillsById = skills.ToDictionary(skill => skill.Id);

        if (skillIds.Any(skillId => !skillsById.ContainsKey(skillId)))
        {
            throw new ArgumentException(
                "One or more skills do not belong to this group.");
        }

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var temporaryDisplayOrder =
            skills.Max(skill => skill.DisplayOrder) + 1;

        for (var index = 0; index < skillIds.Count; index++)
        {
            skillsById[skillIds[index]].DisplayOrder =
                temporaryDisplayOrder + index;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        for (var index = 0; index < skillIds.Count; index++)
        {
            skillsById[skillIds[index]].DisplayOrder = index + 1;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
    public async Task<Skill> AddAsync(Skill skill)
    {
        var displayOrderIsTaken = await dbContext.Skills.AnyAsync(
            existing =>
                existing.SkillGroupId == skill.SkillGroupId &&
                existing.DisplayOrder == skill.DisplayOrder);

        if (displayOrderIsTaken)
        {
            var highestDisplayOrder = await dbContext.Skills
                .Where(existing =>
                    existing.SkillGroupId == skill.SkillGroupId)
                .MaxAsync(existing => (int?)existing.DisplayOrder)
                ?? 0;

            skill.DisplayOrder = highestDisplayOrder + 1;
        }

        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();
        return skill;
    }

    public async Task<bool> DeleteAsync(
        int groupId,
        int skillId)
    {
        var skill = await dbContext.Skills
            .SingleOrDefaultAsync(skill =>
                skill.Id == skillId &&
                skill.SkillGroupId == groupId);

        if (skill is null)
        {
            return false;
        }

        dbContext.Skills.Remove(skill);

        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<Skill?> GetByIdAsync(int id)
    {
        return await dbContext.Skills
           .AsNoTracking()
           .SingleOrDefaultAsync(skill => skill.Id == id);
    }

    public async Task<bool> SkillExistsAsync(Skill skill)
    {
        var exists = await dbContext.Skills.FindAsync(skill.Id);
        if (exists is null) return false;
        return true;
    }

    public async Task<bool> UpdateAsync(
        int groupId,
        Skill skill)
    {
        var existingSkill = await dbContext.Skills
            .SingleOrDefaultAsync(existing =>
                existing.Id == skill.Id &&
                existing.SkillGroupId == groupId);

        if (existingSkill is null)
        {
            return false;
        }

        existingSkill.SkillName = skill.SkillName;
        existingSkill.DisplayOrder = skill.DisplayOrder;

        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateOrderAsync(
        int groupId,
        IReadOnlyList<int> skillIds)
    {
        var skills = await dbContext.Skills
            .Where(skill => skill.SkillGroupId == groupId)
            .ToListAsync();

        if (skills.Count != skillIds.Count)
        {
            return false;
        }

        var skillsById = skills.ToDictionary(skill => skill.Id);

        if (skillIds.Any(skillId => !skillsById.ContainsKey(skillId)))
        {
            return false;
        }

        for (var index = 0; index < skillIds.Count; index++)
        {
            skillsById[skillIds[index]].DisplayOrder = index + 1;
        }

        await dbContext.SaveChangesAsync();

        return true;
    }


}
