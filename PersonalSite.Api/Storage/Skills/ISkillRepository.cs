using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Storage.Skills;

public interface ISkillRepository
{

    Task<Skill> AddAsync(Skill skill);
    Task<bool> UpdateAsync(Skill skill);
    Task<bool> DeleteAsync(int id);
    Task<bool> SkillExistsAsync(Skill skill);
    Task<Skill?> GetByIdAsync(int id);
}
