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
    .UpdateProjectOrderTests;

public sealed class UpdateProjectOrderTests : IntegrationTest
{
    [Fact]
    public async Task AdministratorCanReorderOfficialProjects()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        var first =
            SeedProject(
                actor,
                "First",
                1);

        var second =
            SeedProject(
                actor,
                "Second",
                2);

        var third =
            SeedProject(
                actor,
                "Third",
                3);

        var request =
            new UpdateProjectsOrderRequest(
                [
                    third.Id,
                    first.Id,
                    second.Id
                ]);

        var response =
            await Client.PutAsJsonAsync(
                "/projects/order",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var projects =
            Reader.Query(
                db =>
                    db.Projects
                        .Where(project =>
                            project.Source ==
                            ContentSource.Official)
                        .OrderBy(project =>
                            project.DisplayOrder)
                        .ToList());

        Assert.Equal(
            third.Id,
            projects[0].Id);

        Assert.Equal(
            first.Id,
            projects[1].Id);

        Assert.Equal(
            second.Id,
            projects[2].Id);

        Assert.Equal(
            1,
            projects[0].DisplayOrder);

        Assert.Equal(
            2,
            projects[1].DisplayOrder);

        Assert.Equal(
            3,
            projects[2].DisplayOrder);
    }

    [Fact]
    public async Task RegularUserCanReorderOwnDemoProjects()
    {
        var userId =
            await AuthenticateAsUser();

        var actor =
            new Actor(
                userId,
                UserRole.User);

        var first =
            SeedProject(
                actor,
                "First",
                1);

        var second =
            SeedProject(
                actor,
                "Second",
                2);

        var request =
            new UpdateProjectsOrderRequest(
                [
                    second.Id,
                    first.Id
                ]);

        var response =
            await Client.PutAsJsonAsync(
                "/projects/order",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var projects =
            Reader.Query(
                db =>
                    db.Projects
                        .Where(project =>
                            project.Source ==
                                ContentSource.Demo &&
                            project.Created.UserId ==
                                userId)
                        .OrderBy(project =>
                            project.DisplayOrder)
                        .ToList());

        Assert.Equal(
            second.Id,
            projects[0].Id);

        Assert.Equal(
            first.Id,
            projects[1].Id);
    }

    [Fact]
    public async Task RegularUserCannotReorderOfficialProjects()
    {
        await AuthenticateAsUser();

        var administratorId =
            Reader.Query(
                db =>
                    db.Users
                        .Single(user =>
                            user.Role ==
                            UserRole.Administrator)
                        .Id);

        var administrator =
            new Actor(
                administratorId,
                UserRole.Administrator);

        var first =
            SeedProject(
                administrator,
                "First",
                1);

        var second =
            SeedProject(
                administrator,
                "Second",
                2);

        var request =
            new UpdateProjectsOrderRequest(
                [
                    second.Id,
                    first.Id
                ]);

        var response =
            await Client.PutAsJsonAsync(
                "/projects/order",
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task RegularUserCannotReorderAnotherUsersDemoProjects()
    {
        await AuthenticateAsUser();

        var otherUser =
            new Actor(
                9999,
                UserRole.User);

        var first =
            SeedProject(
                otherUser,
                "Other First",
                1);

        var second =
            SeedProject(
                otherUser,
                "Other Second",
                2);

        var request =
            new UpdateProjectsOrderRequest(
                [
                    second.Id,
                    first.Id
                ]);

        var response =
            await Client.PutAsJsonAsync(
                "/projects/order",
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderRejectsDuplicateProjectIds()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        var first =
            SeedProject(
                actor,
                "First",
                1);

        var second =
            SeedProject(
                actor,
                "Second",
                2);

        var request =
            new UpdateProjectsOrderRequest(
                [
                    first.Id,
                    first.Id,
                    second.Id
                ]);

        var response =
            await Client.PutAsJsonAsync(
                "/projects/order",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderRequiresCompleteProjectList()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        var first =
            SeedProject(
                actor,
                "First",
                1);

        SeedProject(
            actor,
            "Second",
            2);

        var request =
            new UpdateProjectsOrderRequest(
                [first.Id]);

        var response =
            await Client.PutAsJsonAsync(
                "/projects/order",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderRejectsUnknownProject()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        var project =
            SeedProject(
                actor,
                "Existing",
                1);

        var request =
            new UpdateProjectsOrderRequest(
                [
                    project.Id,
                    9999
                ]);

        var response =
            await Client.PutAsJsonAsync(
                "/projects/order",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private Project SeedProject(
        Actor actor,
        string title,
        int displayOrder)
    {
        var project =
            Project.Create(
                actor,
                new ProjectTitle(title),
                new ProjectDescription(
                    $"{title} description"),
                displayOrder,
                new Url(
                    $"https://github.com/example/{title.ToLowerInvariant()}"),
                null,
                false,
                []);

        Writer.Seed(
            db =>
                db.Projects.Add(project));

        return project;
    }
}