using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Skills;
using PersonalSite.Api.Storage.Skills;

namespace PersonalSite.Api.Application.Skills.UpdateSkillGroup;

public class UpdateSkillGroupCommandHandler(
    ISkillGroupRepository skillGroupRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int groupId,
        UpdateSkillGroupRequest request)
    {
        Permissions.EnsureCanManage(actor);

        var skillGroup = new SkillGroup
        {
            Id = groupId,
            Name = new SkillGroupName(request.Name),
            DisplayOrder = request.DisplayOrder
        };

        return await skillGroupRepository.UpdateAsync(
            skillGroup);
    }
}