using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Skills.GetSkillDetails;

public class GetSkillDetailsQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetSkillDetailsResponse?> Execute(
        Actor actor,
        int groupId,
        int skillId)
    {
        Permissions.EnsureCanViewDirectory(actor);

        return await dbContext.Skills
            .AsNoTracking()
            .Where(skill =>
                skill.Id == skillId &&
                skill.SkillGroupId == groupId)
            .Select(skill =>
                new GetSkillDetailsResponse
                {
                    Id = skill.Id,
                    SkillGroupId = skill.SkillGroupId,
                    Name = skill.SkillName.Value,
                    DisplayOrder = skill.DisplayOrder
                })
            .FirstOrDefaultAsync();
    }
}