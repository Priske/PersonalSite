using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Application.Skills.GetSkillSummaries;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Skills.GetSkillSummeries;

public class GetSkillSummariesQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetSkillSummariesResponse> Execute(
        Actor actor,
        int groupId)
    {
        Permissions.EnsureCanViewDirectory(actor);

        var skills = await dbContext.Skills
            .AsNoTracking()
            .Where(skill =>
                skill.SkillGroupId == groupId)
            .OrderBy(skill =>
                skill.DisplayOrder)
            .Select(skill =>
                new SkillSummary
                {
                    Id = skill.Id,
                    Name = skill.SkillName.Value,
                    DisplayOrder = skill.DisplayOrder
                })
            .ToListAsync();

        return new GetSkillSummariesResponse
        {
            Items = skills
        };
    }
}