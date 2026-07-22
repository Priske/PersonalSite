using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Storage.Skills;


namespace PersonalSite.Api.Application.Skills.DeleteSkillgroup;

public class DeleteSkillGroupCommandHandler(
    ISkillGroupRepository skillGroupRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int groupId)
    {
        Permissions.EnsureCanManage(actor);

        return await skillGroupRepository.DeleteAsync(
            groupId);
    }
}