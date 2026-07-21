
using Microsoft.EntityFrameworkCore;
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
}