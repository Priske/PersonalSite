using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Projects.GetProjectDetails;

public class GetProjectDetailsQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetProjectDetailsResponse?> Execute(int id)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(project => project.Id == id)
            .Select(project =>
                new GetProjectDetailsResponse
                {
                    Id = project.Id,
                    Title = project.Title.Value,
                    Description = project.Description.Value,
                    RepositoryUrl = project.RepositoryUrl.Value,
                    LiveUrl = project.LiveUrl == null
                        ? null
                        : project.LiveUrl.Value,
                    IsFeatured = project.IsFeatured,
                    DisplayOrder = project.DisplayOrder
                })
            .FirstOrDefaultAsync();
    }
}