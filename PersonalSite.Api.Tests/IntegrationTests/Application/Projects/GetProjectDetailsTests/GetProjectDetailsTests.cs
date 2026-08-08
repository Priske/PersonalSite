using System.Net;

using PersonalSite.Api.Application.Projects.GetProjectDetails;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application
    .Projects.GetProjectDetailsTests;

public class GetProjectDetailsTests : IntegrationTest
{
    [Fact]
    public async Task GetProjectDetailsReturnsProject()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var project = Project.Create(
            new Actor(1, UserRole.Administrator),
            new ProjectTitle("Personal site"),
            new ProjectDescription("My personal portfolio site"),
            1,
            new Url("https://github.com/example/personal-site"),
            new Url("https://example.com"),
            true,
            []);

        Writer.Seed(db => db.Projects.Add(project));

        var response =
            await Client.GetAsync($"/projects/{project.Id}");

        var content =
            await response.ReadJsonAs<GetProjectDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(project.Id, content.Id);
        Assert.Equal("Personal site", content.Title);
        Assert.Equal(
            "My personal portfolio site",
            content.Description);
        Assert.Equal(
            "https://github.com/example/personal-site",
            content.RepositoryUrl);
        Assert.Equal(
            "https://example.com/",
            content.LiveUrl);
        Assert.True(content.IsFeatured);
        Assert.Equal(1, content.DisplayOrder);
    }

    [Fact]
    public async Task GetProjectDetailsWithoutLiveUrlReturnsNull()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var project = Project.Create(
            new Actor(1, UserRole.Administrator),
            new ProjectTitle("Personal site"),
            new ProjectDescription("My personal portfolio site"),
            1,
            new Url("https://github.com/example/personal-site"),
            null,
            false,
            []);

        Writer.Seed(db => db.Projects.Add(project));

        var response =
            await Client.GetAsync($"/projects/{project.Id}");

        var content =
            await response.ReadJsonAs<GetProjectDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Null(content.LiveUrl);
    }

    [Fact]
    public async Task GetUnknownProjectReturnsNotFound()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var response =
            await Client.GetAsync("/projects/9999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}