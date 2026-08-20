using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Skills;
using PersonalSite.Api.Storage.Skills;

namespace PersonalSite.Api.Application.Skills.CreateSkillGroup;

public class CreateSkillGroupCommandHandler(
    ISkillGroupRepository skillGroupRepository) : IHandler
{
    public async Task<CreateSkillGroupResponse> Execute(
        Actor actor,
        CreateSkillGroupRequest request)
    {
        Permissions.EnsureCanManage(actor);

        var skillGroup = SkillGroup.Create(
            actor,
            new SkillGroupName(request.Name),
            request.DisplayOrder);

        var savedGroup = await skillGroupRepository.AddAsync(skillGroup);

        return new CreateSkillGroupResponse
        {
            Id = savedGroup.Id,
            Name = savedGroup.Name.Value,
            DisplayOrder = savedGroup.DisplayOrder
        };
    }
}
