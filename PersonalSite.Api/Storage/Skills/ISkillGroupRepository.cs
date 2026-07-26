using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Storage.Skills;

public interface ISkillGroupRepository
{

    Task<SkillGroup> AddAsync(SkillGroup skillgroup);
    Task<bool> UpdateAsync(SkillGroup skillgroup);
    Task<bool> DeleteAsync(int id);
    Task<bool> SkillExistsAsync(SkillGroup skillgroup);
    Task<SkillGroup?> GetByIdAsync(int id);
    Task<bool> UpdateOrderAsync(IReadOnlyList<int> skillGroupIds, CancellationToken cancellationToken);
}
