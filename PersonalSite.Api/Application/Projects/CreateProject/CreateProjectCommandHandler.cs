using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage.Projects;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.Projects.CreateProject;

public class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    ITagRepository tagRepository) : IHandler
{
    public async Task<CreateProjectResponse> Execute(
        CreateProjectRequest request,
        Actor actor)
    {
        ProjectPermissions.EnsureCanManage(actor);

        var liveUrl = string.IsNullOrWhiteSpace(request.LiveUrl)
            ? null
            : new Url(request.LiveUrl);

        var tagIds = request.TagIds
            .Distinct()
            .ToList();

        var tags = await tagRepository.GetByIdsAsync(tagIds);

        if (tags.Count != tagIds.Count)
        {
            throw new DomainException(
                "One or more selected tags do not exist.");
        }

        var project = new Project
        {
            Title = new ProjectTitle(request.Title),
            Description = new ProjectDescription(request.Description),
            DisplayOrder = request.DisplayOrder,
            RepositoryUrl = new Url(request.RepositoryUrl),
            LiveUrl = liveUrl,
            IsFeatured = request.IsFeatured,
            Tags = tags.ToList()
        };

        var savedProject =
            await projectRepository.AddAsync(project);

        return new CreateProjectResponse
        {
            Id = savedProject.Id,
            Title = savedProject.Title,
            Description = savedProject.Description,
            RepositoryUrl = savedProject.RepositoryUrl,
            IsFeatured = savedProject.IsFeatured,
            DisplayOrder = savedProject.DisplayOrder,
            LiveUrl = savedProject.LiveUrl?.Value,
            Tags = savedProject.Tags
                .Select(tag => tag.Name.Value)
                .ToList()
        };
    }
}