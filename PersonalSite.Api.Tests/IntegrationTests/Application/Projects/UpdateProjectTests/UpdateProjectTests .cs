using System.Net;
using System.Net.Http.Json;
using PersonalSite.Api.Application.Projects.UpdateProjects;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application
    .Projects.UpdateProjectTests;

public class UpdateProjectTests : IntegrationTest
{
    [Fact]
    public async Task UpdateProjectUpdatesProject()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var project =
            SeedProject(
                new Actor(
                    userId,
                    UserRole.Administrator));

        var request =
            new UpdateProjectRequest
            {
                Title = "Updated project",
                Description = "Updated description",
                RepositoryUrl =
                    "https://github.com/example/updated-project",
                LiveUrl =
                    "https://updated.example.com",
                IsFeatured = true,
                TagIds = []
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updatedProject =
            Reader.Query(
                db =>
                    db.Projects.Single(
                        projectInDb =>
                            projectInDb.Id ==
                            project.Id));

        Assert.NotNull(updatedProject);

        Assert.Equal(
            "Updated project",
            updatedProject.Title.Value);

        Assert.Equal(
            "Updated description",
            updatedProject.Description.Value);

        Assert.Equal(
            "https://github.com/example/updated-project",
            updatedProject.RepositoryUrl.Value);

        Assert.Equal(
            "https://updated.example.com/",
            updatedProject.LiveUrl?.Value);

        Assert.True(
            updatedProject.IsFeatured);

        Assert.Equal(
            1,
            updatedProject.DisplayOrder);
    }

    [Fact]
    public async Task UpdateProjectWithoutLiveUrlStoresNull()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var project =
            SeedProject(
                new Actor(
                    userId,
                    UserRole.Administrator),
                liveUrl:
                    "https://old.example.com");

        var request =
            new UpdateProjectRequest
            {
                Title = "Updated project",
                Description = "Updated description",
                RepositoryUrl =
                    "https://github.com/example/updated-project",
                LiveUrl = null,
                IsFeatured = false,
                TagIds = []
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updatedProject =
            Reader.Query(
                db =>
                    db.Projects.Single(
                        projectInDb =>
                            projectInDb.Id ==
                            project.Id));

        Assert.NotNull(updatedProject);
        Assert.Null(updatedProject.LiveUrl);
    }

    [Fact]
    public async Task UpdateProjectWithWhitespaceLiveUrlStoresNull()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var project =
            SeedProject(
                new Actor(
                    userId,
                    UserRole.Administrator),
                liveUrl:
                    "https://old.example.com");

        var request =
            new UpdateProjectRequest
            {
                Title = "Updated project",
                Description = "Updated description",
                RepositoryUrl =
                    "https://github.com/example/updated-project",
                LiveUrl = "   ",
                IsFeatured = false,
                TagIds = []
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updatedProject =
            Reader.Query(
                db =>
                    db.Projects.Single(
                        projectInDb =>
                            projectInDb.Id ==
                            project.Id));

        Assert.NotNull(updatedProject);
        Assert.Null(updatedProject.LiveUrl);
    }

    [Fact]
    public async Task UpdateUnknownProjectReturnsNotFound()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var request =
            ValidRequest();

        var response =
            await Client.PutAsJsonAsync(
                "/projects/9999",
                request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task RegularUserCannotUpdateOfficialProject()
    {
        await AuthenticateAsUser(
            UserRole.User);

        var administratorId =
            Reader.Query(
                db =>
                    db.Users
                        .Single(
                            user =>
                                user.Role ==
                                UserRole.Administrator)
                        .Id);

        var project =
            SeedProject(
                new Actor(
                    administratorId,
                    UserRole.Administrator));

        var request =
            ValidRequest();

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);

        var unchangedProject =
            Reader.Query(
                db =>
                    db.Projects.Single(
                        projectInDb =>
                            projectInDb.Id ==
                            project.Id));

        Assert.NotNull(unchangedProject);

        Assert.Equal(
            "Original project",
            unchangedProject.Title.Value);
    }

    [Fact]
    public async Task RegularUserCanUpdateOwnDemoProject()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.User);

        var project =
            SeedProject(
                new Actor(
                    userId,
                    UserRole.User));

        var request =
            ValidRequest();

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updatedProject =
            Reader.Query(
                db =>
                    db.Projects.Single(
                        projectInDb =>
                            projectInDb.Id ==
                            project.Id));

        Assert.Equal(
            "Updated project",
            updatedProject.Title.Value);
    }

    [Fact]
    public async Task UpdateProjectWithWhitespaceTitleReturnsBadRequest()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var project =
            SeedProject(
                new Actor(
                    userId,
                    UserRole.Administrator));

        var request =
            ValidRequest();

        request.Title = "   ";

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateProjectWithInvalidRepositoryUrlReturnsBadRequest()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var project =
            SeedProject(
                new Actor(
                    userId,
                    UserRole.Administrator));

        var request =
            ValidRequest();

        request.RepositoryUrl =
            "not-a-url";

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateProjectWithInvalidLiveUrlReturnsBadRequest()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var project =
            SeedProject(
                new Actor(
                    userId,
                    UserRole.Administrator));

        var request =
            ValidRequest();

        request.LiveUrl =
            "not-a-url";

        var response =
            await Client.PutAsJsonAsync(
                $"/projects/{project.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private Project SeedProject(
        Actor actor,
        string? liveUrl = null)
    {
        var project =
            Project.Create(
                actor,
                new ProjectTitle(
                    "Original project"),
                new ProjectDescription(
                    "Original description"),
                displayOrder: 1,
                new Url(
                    "https://github.com/example/original-project"),
                liveUrl is null
                    ? null
                    : new Url(liveUrl),
                isFeatured: false,
                tags: []);

        Writer.Seed(
            db => db.Projects.Add(project));

        return project;
    }

    private static UpdateProjectRequest ValidRequest()
    {
        return new UpdateProjectRequest
        {
            Title = "Updated project",
            Description = "Updated description",
            RepositoryUrl =
                "https://github.com/example/updated-project",
            LiveUrl =
                "https://updated.example.com",
            IsFeatured = true,
            TagIds = []
        };
    }
}