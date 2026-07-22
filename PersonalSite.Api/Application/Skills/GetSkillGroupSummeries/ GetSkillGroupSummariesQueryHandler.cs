using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Skills.GetSkillGroupSummeries;

public class GetSkillGroupSummariesQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetSkillGroupSummariesResponse> Execute()
    {

        var groups = await dbContext.SkillGroups
            .AsNoTracking()
            .OrderBy(group => group.DisplayOrder)
            .Select(group => new SkillGroupSummary
            {
                Id = group.Id,
                Name = group.Name.Value,
                DisplayOrder = group.DisplayOrder
            })
            .ToListAsync();

        return new GetSkillGroupSummariesResponse
        {
            Items = groups
        };
    }
}