using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Skills;
using PersonalSite.Api.Storage.Skills;

namespace PersonalSite.Api.Application.Skills.CreateSkill;

public class CreateSkillCommandHandler(
    ISkillRepository skillRepository,
    ISkillGroupRepository skillGroupRepository) : IHandler
{
    public async Task<CreateSkillResponse> Execute(
        Actor actor,
        int groupId,
        CreateSkillRequest request)
    {
        Permissions.EnsureCanManage(actor);

        var group = await skillGroupRepository.GetByIdAsync(groupId);

        if (group is null)
        {
            throw new NotFoundException("Skill group not found.");
        }

        var skill = new Skill
        {
            SkillGroupId = groupId,
            SkillName = new SkillName(request.Name),
            DisplayOrder = request.DisplayOrder
        };

        var saved = await skillRepository.AddAsync(skill);

        return new CreateSkillResponse
        {
            Id = saved.Id,
            SkillGroupId = saved.SkillGroupId,
            Name = saved.SkillName,
            DisplayOrder = saved.DisplayOrder
        };
    }
}