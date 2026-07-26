using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage.Projects;

namespace PersonalSite.Api.Application.Projects.CreateProject;

public class CreateProjectCommandHandler(
    IProjectRepository projectRepository) : IHandler
{
    public async Task<CreateProjectResponse> Execute(
        CreateProjectRequest request,
        Actor actor)
    {
        ProjectPermissions.EnsureCanManage(actor);

        var liveUrl = string.IsNullOrWhiteSpace(request.LiveUrl)
            ? null
            : new Url(request.LiveUrl);

        var project = new Project
        {
            Title = new ProjectTitle(request.Title),
            Description = new ProjectDiscription(request.Discription),
            DisplayOrder = request.DisplayOrder,
            RepositoryUrl = new Url(request.RepositoryUrl),
            LiveUrl = liveUrl,
            IsFeatured = request.IsFeatured,
            Tags = request.Tags;
        };

        var savedProject =
            await projectRepository.AddAsync(project);

        return new CreateProjectResponse
        {
            Id = savedProject.Id,
            Title = savedProject.Title,
            Discription = savedProject.Description,
            RepositoryUrl = savedProject.RepositoryUrl,
            IsFeatured = savedProject.IsFeatured,
            DisplayOrder = savedProject.DisplayOrder,
            LiveUrl = savedProject.LiveUrl?.Value,
            Tags = savedProject.Tags

        };
    }
}