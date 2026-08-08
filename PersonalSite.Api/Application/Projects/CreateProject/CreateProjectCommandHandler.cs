using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage.Projects;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.Projects.CreateProject;

public class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    ITagRepository tagRepository,
    ILogger<CreateProjectCommandHandler> logger) : IHandler
{
    public async Task<CreateProjectResponse> Execute(
        CreateProjectRequest request,
        Actor actor,
        CancellationToken cancellationToken)
    {
        ProjectPermissions.EnsureCanCreate(actor);

        var liveUrl = string.IsNullOrWhiteSpace(request.LiveUrl)
            ? null
            : new Url(request.LiveUrl);

        var tagIds = request.TagIds
            .Distinct()
            .ToList();

        var tags = await tagRepository.GetByIdsAsync(
            tagIds,
            cancellationToken);

        if (tags.Count != tagIds.Count)
        {
            throw new DomainException(
                "One or more selected tags do not exist.");
        }

        var project = Project.Create(
            actor,
            new ProjectTitle(request.Title),
            new ProjectDescription(request.Description),
            request.DisplayOrder,
            new Url(request.RepositoryUrl),
            liveUrl,
            request.IsFeatured,
            tags.ToList());

        var savedProject = await projectRepository.AddAsync(
            project,
            cancellationToken);

        logger.LogInformation(
            "Project {ProjectId} created by actor {ActorId}",
            savedProject.Id,
            actor.UserId);

        return new CreateProjectResponse
        {
            Id = savedProject.Id,
            Title = savedProject.Title.Value,
            Description = savedProject.Description.Value,
            RepositoryUrl = savedProject.RepositoryUrl.Value,
            LiveUrl = savedProject.LiveUrl?.Value,
            IsFeatured = savedProject.IsFeatured,
            DisplayOrder = savedProject.DisplayOrder,

            Tags = savedProject.Tags
                .Select(tag => tag.Name.Value)
                .ToList(),

            Source = savedProject.Source.ToString(),

            CreatedByUserId = savedProject.Created.UserId,
            CreatedAt = savedProject.Created.At,

            LastEditedByUserId = savedProject.Edited.UserId,
            LastEditedAt = savedProject.Edited.At
        };
    }
}