using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Tags.GetTagDetails;

public sealed class GetTagDetailsQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetTagDetailsResponse?> Execute(
        int id, CancellationToken cancellationToken)
    {
        return await dbContext.Tags
            .AsNoTracking()
            .Where(tag => tag.Id == id)
            .Select(tag => new GetTagDetailsResponse
            {
                Id = tag.Id,
                Name = tag.Name.Value,
                Projects = tag.Projects
                .OrderBy(project => project.Title)
                .Select(project => new TagProjectResponse(
                    project.Id,
                    project.Title))
                .ToList()

            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}