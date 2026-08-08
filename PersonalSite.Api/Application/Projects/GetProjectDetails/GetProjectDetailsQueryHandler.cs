using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Application.Tags.GetTagSummaries;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Projects.GetProjectDetails;

public class GetProjectDetailsQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetProjectDetailsResponse?> Execute(
        int id,
        Actor actor,
        CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Tags)
            .FirstOrDefaultAsync(
                project => project.Id == id,
                cancellationToken);

        if (project is null)
        {
            return null;
        }

        ProjectPermissions.EnsureCanManage(
            actor,
            project);

        return new GetProjectDetailsResponse
        {
            Id = project.Id,
            Title = project.Title.Value,
            Description = project.Description.Value,
            RepositoryUrl = project.RepositoryUrl.Value,
            LiveUrl = project.LiveUrl?.Value,
            IsFeatured = project.IsFeatured,
            DisplayOrder = project.DisplayOrder,

            Tags = project.Tags
                .OrderBy(tag => tag.Name.Value)
                .Select(tag => new TagSummary
                {
                    Id = tag.Id,
                    Name = tag.Name.Value
                })
                .ToList()
        };
    }
}