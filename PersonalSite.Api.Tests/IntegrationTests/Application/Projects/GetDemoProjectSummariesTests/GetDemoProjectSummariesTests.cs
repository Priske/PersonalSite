using System.Net;

using PersonalSite.Api.Application.Projects.GetProjectSummeries;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Projects
    .GetDemoProjectSummariesTests;

public sealed class GetDemoProjectSummariesTests : IntegrationTest
{
    [Fact]
    public async Task GetDemoProjectsReturnsOnlyLoggedInUsersDemoProjects()
    {
        var userId =
            await AuthenticateAsUser();

        var user =
            new Actor(
                userId,
                UserRole.User);

        var otherUser =
            new Actor(
                9999,
                UserRole.User);

        var administratorId =
            Reader.Query(
                db =>
                    db.Users
                        .Single(current =>
                            current.Role ==
                            UserRole.Administrator)
                        .Id);

        var administrator =
            new Actor(
                administratorId,
                UserRole.Administrator);

        SeedProject(
            user,
            "My Demo",
            1);

        SeedProject(
            otherUser,
            "Someone Else",
            1);

        SeedProject(
            administrator,
            "Official",
            1);

        var response =
            await Client.GetAsync(
                "/demo-projects");

        var result =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        var project =
            Assert.Single(
                result.Items);

        Assert.Equal(
            "My Demo",
            project.Title);

        Assert.Equal(
            "Demo",
            project.Source);

        Assert.Equal(
            userId,
            project.CreatedByUserId);
    }

    [Fact]
    public async Task GetDemoProjectsOrdersProjectsByDisplayOrder()
    {
        var userId =
            await AuthenticateAsUser();

        var actor =
            new Actor(
                userId,
                UserRole.User);

        SeedProject(
            actor,
            "Third",
            3);

        SeedProject(
            actor,
            "First",
            1);

        SeedProject(
            actor,
            "Second",
            2);

        var response =
            await Client.GetAsync(
                "/demo-projects");

        var result =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            "First",
            result.Items[0].Title);

        Assert.Equal(
            "Second",
            result.Items[1].Title);

        Assert.Equal(
            "Third",
            result.Items[2].Title);
    }

    [Fact]
    public async Task GetDemoProjectsCanSearchWithinOwnProjects()
    {
        var userId =
            await AuthenticateAsUser();

        var actor =
            new Actor(
                userId,
                UserRole.User);

        SeedProject(
            actor,
            "Portfolio",
            1);

        SeedProject(
            actor,
            "Book Tracker",
            2);

        var response =
            await Client.GetAsync(
                "/demo-projects?search=Book");

        var result =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        var project =
            Assert.Single(
                result.Items);

        Assert.Equal(
            "Book Tracker",
            project.Title);
    }

    [Fact]
    public async Task GetDemoProjectsRequiresAuthentication()
    {
        var response =
            await Client.GetAsync(
                "/demo-projects");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    private void SeedProject(
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
                    $"https://github.com/example/{title.Replace(" ", "-").ToLowerInvariant()}"),
                null,
                false,
                []);

        Writer.Seed(
            db =>
                db.Projects.Add(project));
    }
}