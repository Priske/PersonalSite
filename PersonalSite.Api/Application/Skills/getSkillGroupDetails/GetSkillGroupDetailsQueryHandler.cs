using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Application.Skills.GetSkillSummeries;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Skills.GetSkillGroupDetails;

public class GetSkillGroupDetailsQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetSkillGroupDetailsResponse?> Execute(
        int groupId)
    {
        return await dbContext.SkillGroups
            .AsNoTracking()
            .Where(group => group.Id == groupId)
            .Select(group =>
                new GetSkillGroupDetailsResponse
                {
                    Id = group.Id,
                    Name = group.Name.Value,
                    DisplayOrder = group.DisplayOrder,

                    Skills = group.Skills
                        .OrderBy(skill => skill.DisplayOrder)
                        .Select(skill =>
                            new SkillSummary
                            {
                                Id = skill.Id,
                                Name = skill.SkillName.Value,
                                DisplayOrder = skill.DisplayOrder
                            })
                        .ToList()
                })
            .FirstOrDefaultAsync();
    }
}