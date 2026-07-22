using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Skills;
using PersonalSite.Api.Storage.Skills;

namespace PersonalSite.Api.Application.Skills.UpdateSkill;

public class UpdateSkillCommandHandler(
    ISkillRepository skillRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int groupId,
        int skillId,
        UpdateSkillRequest request)
    {
        Permissions.EnsureCanManage(actor);

        var skill = new Skill
        {
            Id = skillId,
            SkillGroupId = groupId,
            SkillName = new SkillName(request.Name),
            DisplayOrder = request.DisplayOrder
        };

        return await skillRepository.UpdateAsync(
            groupId,
            skill);
    }
}