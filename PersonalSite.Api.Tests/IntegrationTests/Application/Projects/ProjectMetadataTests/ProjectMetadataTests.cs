using System.Net;
using System.Net.Http.Json;

using PersonalSite.Api.Application.Projects.UpdateProjects;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Projects
    .ProjectMetadataTests;

public sealed class ProjectMetadataTests : IntegrationTest
{
    [Fact]
    public void CreateProjectSetsCreatedAndEditedToSameActorAndTime()
    {
        var actor =
            new Actor(
                42,
                UserRole.User);

        var project =
            Project.Create(
                actor,
                new ProjectTitle("Project"),
                new ProjectDescription(
                    "Description"),
                1,
                new Url(
                    "https://github.com/example/project"),
                null,
                false,
                []);

        Assert.Equal(
            actor.UserId,
            project.Created.UserId);

        Assert.Equal(
            actor.UserId,
            project.Edited.UserId);

        Assert.Equal(
            project.Created.At,
            project.Edited.At);

        Assert.Equal(
            ContentSource.Demo,
            project.Source);
    }

    [Fact]
    public async Task UpdateProjectPreservesCreatedMetadata()
    {
        var userId =
            await AuthenticateAsUser();

        var actor =
            new Actor(
                userId,
                UserRole.User);

        var project =
            SeedProject(actor);

        var createdUserId =
            project.Created.UserId;

        var createdAt =
            project.Created.At;

        var request =
            ValidRequest();

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updated =
            Reader.Query(
                db =>
                    db.Projects.Single(
                        current =>
                            current.Id ==
                            project.Id));

        Assert.Equal(
            createdUserId,
            updated.Created.UserId);
        Assert.InRange(
            updated.Created.At,
            createdAt.AddMicroseconds(-1),
            createdAt.AddMicroseconds(1));
    }

    [Fact]
    public async Task UpdateProjectChangesEditedMetadata()
    {
        var userId =
            await AuthenticateAsUser();

        var actor =
            new Actor(
                userId,
                UserRole.User);

        var project =
            SeedProject(actor);

        var originalEditedAt =
            project.Edited.At;

        await Task.Delay(10);

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                ValidRequest());

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updated =
            Reader.Query(
                db =>
                    db.Projects.Single(
                        current =>
                            current.Id ==
                            project.Id));

        Assert.Equal(
            userId,
            updated.Edited.UserId);

        Assert.True(
            updated.Edited.At >
            originalEditedAt);
    }

    [Fact]
    public async Task UpdateProjectDoesNotChangeSource()
    {
        var userId =
            await AuthenticateAsUser();

        var actor =
            new Actor(
                userId,
                UserRole.User);

        var project =
            SeedProject(actor);

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                ValidRequest());

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updated =
            Reader.Query(
                db =>
                    db.Projects.Single(
                        current =>
                            current.Id ==
                            project.Id));

        Assert.Equal(
            ContentSource.Demo,
            updated.Source);
    }

    private Project SeedProject(
        Actor actor)
    {
        var project =
            Project.Create(
                actor,
                new ProjectTitle(
                    "Original"),
                new ProjectDescription(
                    "Original description"),
                1,
                new Url(
                    "https://github.com/example/original"),
                null,
                false,
                []);

        Writer.Seed(
            db =>
                db.Projects.Add(project));

        return project;
    }

    private static UpdateProjectRequest ValidRequest()
    {
        return new UpdateProjectRequest
        {
            Title =
                "Updated",

            Description =
                "Updated description",

            RepositoryUrl =
                "https://github.com/example/updated",

            LiveUrl =
                null,

            IsFeatured =
                false,

            TagIds =
                []
        };
    }
}