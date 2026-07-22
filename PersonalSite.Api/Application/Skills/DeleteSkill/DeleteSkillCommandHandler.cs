using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Storage.Skills;

namespace PersonalSite.Api.Application.Skills.DeleteSkill;

public class DeleteSkillCommandHandler(
    ISkillRepository skillRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int groupId,
        int skillId)
    {
        Permissions.EnsureCanManage(actor);

        return await skillRepository.DeleteAsync(
            groupId,
            skillId);
    }
}