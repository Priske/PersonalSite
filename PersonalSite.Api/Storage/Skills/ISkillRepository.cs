using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Storage.Skills;

public interface ISkillRepository
{

    Task<Skill> AddAsync(Skill skill);
    Task<bool> UpdateAsync(int groupId, Skill skill);
    Task<bool> DeleteAsync(int groupId, int skillId);
    Task<bool> SkillExistsAsync(Skill skill);
    Task<Skill?> GetByIdAsync(int id);
    Task<bool> UpdateOrderAsync(int groupId, IReadOnlyList<int> skillIds);
}
