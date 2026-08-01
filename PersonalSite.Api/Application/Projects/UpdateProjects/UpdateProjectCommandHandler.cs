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
        ProjectPermissions.EnsureCanManage(actor);

        var requestedTagIds = request.TagIds
            .Distinct()
            .ToArray();

        var tags = await tagRepository.GetByIdsAsync(requestedTagIds, cancellationToken);

        if (tags.Count != requestedTagIds.Length)
        {
            throw new DomainException(
                "One or more selected tags do not exist.");
        }

        var project = new Project
        {
            Id = id,
            Title = new ProjectTitle(request.Title),
            Description =
                new ProjectDescription(request.Description),
            RepositoryUrl =
                new Url(request.RepositoryUrl),
            LiveUrl =
                string.IsNullOrWhiteSpace(request.LiveUrl)
                    ? null
                    : new Url(request.LiveUrl),
            IsFeatured = request.IsFeatured,
            Tags = tags.ToList()
        };

        return await projectRepository.UpdateAsync(
            project,
            cancellationToken);
    }
}