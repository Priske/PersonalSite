
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Storage.Skills;

public class EfSkillRepository(AppDbContext dbContext) : ISkillRepository
{
    public async Task<Skill> AddAsync(Skill skill)
    {
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
}