using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Application.Skills.GetSkillSummaries;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Skills.GetSkillSummeries;

public class GetSkillSummariesQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetSkillSummariesResponse> Execute(
    int groupId)
    {
        var skills = await dbContext.Skills
            .Where(skill =>
                skill.SkillGroupId == groupId)
            .OrderBy(skill =>
                skill.DisplayOrder)
            .Select(skill => new SkillSummary
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