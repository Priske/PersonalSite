using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage.Projects;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.Projects.UpdateProjects;

public class UpdateProjectCommandHandler(
    IProjectRepository projectRepository,
    ITagRepository tagRepository) : IHandler
{
    public async Task<bool> Execute(
    Actor actor,
    int id,
    UpdateProjectRequest request,
    CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (project is null)
        {
            return false;
        }

        ProjectPermissions.EnsureCanManage(actor, project);

        var requestedTagIds = request.TagIds
            .Distinct()
            .ToArray();

        var tags = await tagRepository.GetByIdsAsync(
            requestedTagIds,
            cancellationToken);

        if (tags.Count != requestedTagIds.Length)
        {
            throw new DomainException(
                "One or more selected tags do not exist.");
        }

        project.Title = new ProjectTitle(request.Title);
        project.Description = new ProjectDescription(request.Description);
        project.RepositoryUrl = new Url(request.RepositoryUrl);

        project.LiveUrl = string.IsNullOrWhiteSpace(request.LiveUrl)
            ? null
            : new Url(request.LiveUrl);

        project.IsFeatured = request.IsFeatured;
        project.Tags = tags.ToList();

        project.Edited = new Change(
            actor.UserId,
            DateTimeOffset.UtcNow);

        return await projectRepository.UpdateAsync(
            project,
            cancellationToken);
    }
}