using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage.Projects;

namespace PersonalSite.Api.Application.Projects.UpdateProject;

public class UpdateProjectCommandHandler(
    IProjectRepository projectRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        UpdateProjectRequest request)
    {
        ProjectPermissions.EnsureCanManage(actor);

        var project = new Project
        {
            Id = id,
            Title = new ProjectTitle(request.Title),
            Description =
                new ProjectDiscription(request.Discription),
            RepositoryUrl =
                new Url(request.RepositoryUrl),
            LiveUrl =
                string.IsNullOrWhiteSpace(request.LiveUrl)
                    ? null
                    : new Url(request.LiveUrl),
            IsFeatured = request.IsFeatured,
            DisplayOrder = request.DisplayOrder
        };

        return await projectRepository.UpdateAsync(project);
    }
}