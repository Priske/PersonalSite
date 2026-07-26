using System.Net;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Projects.DeleteProjectTests;

public class DeleteProjectTests : IntegrationTest
{
    [Fact]
    public async Task DeleteProjectDeletesProject()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var project = new Project
        {
            Title = new ProjectTitle("Portfolio"),
            Description = new ProjectDescription("My portfolio"),
            RepositoryUrl = new Url("https://github.com/example/portfolio"),
            DisplayOrder = 1,
            IsFeatured = true
        };

        Writer.Seed(db => db.Projects.Add(project));

        var response =
            await Client.DeleteAsync($"/projects/{project.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var deleted =
            Reader.Query(db => db.Find<Project>(project.Id));

        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteUnknownProjectReturnsNotFound()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var response =
            await Client.DeleteAsync("/projects/9999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task RegularUserCannotDeleteProject()
    {
        await AuthenticateAsUser();

        var project = new Project
        {
            Title = new ProjectTitle("Portfolio"),
            Description = new ProjectDescription("My portfolio"),
            RepositoryUrl = new Url("https://github.com/example/portfolio"),
            DisplayOrder = 1,
            IsFeatured = true
        };

        Writer.Seed(db => db.Projects.Add(project));

        var response =
            await Client.DeleteAsync($"/projects/{project.Id}");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);

        var existing =
            Reader.Query(db => db.Find<Project>(project.Id));

        Assert.NotNull(existing);
    }
}