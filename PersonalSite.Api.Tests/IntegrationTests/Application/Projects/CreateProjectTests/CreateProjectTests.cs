using System.Net;
using System.Net.Http.Json;

using PersonalSite.Api.Application.Projects.CreateProject;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Projects.CreateProjectTests;

public sealed class CreateProjectTests : IntegrationTest
{
    [Fact]
    public async Task PostProjectCreatesProject()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var tagId = SeedTag();

        var request = new CreateProjectRequest
        {
            Title = "Personal Site",
            Description = "My personal portfolio website",
            RepositoryUrl = "https://github.com/example/personal-site",
            LiveUrl = "https://example.com",
            IsFeatured = true,
            TagIds = [tagId]
        };

        var response = await Client.PostAsJsonAsync(
            "/projects",
            request);

        var created = await response.ReadJsonAs<CreateProjectResponse>(
            HttpStatusCode.Created);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        Assert.NotNull(created);
        Assert.True(created.Id > 0);

        Assert.Equal(
            "Personal Site",
            created.Title);

        Assert.Equal(
            "My personal portfolio website",
            created.Description);

        Assert.Equal(
            "https://github.com/example/personal-site",
            created.RepositoryUrl);

        Assert.Equal(
            "https://example.com/",
            created.LiveUrl);

        Assert.True(created.IsFeatured);
        Assert.Equal(1, created.DisplayOrder);

        Assert.Equal(
            ContentSource.Official.ToString(),
            created.Source);

        Assert.NotNull(created.CreatedByUserId);
        Assert.NotNull(created.LastEditedByUserId);

        var project = Reader.Query(
            context => context.Find<Project>(created.Id));

        Assert.NotNull(project);

        Assert.Equal(
            "Personal Site",
            project.Title);

        Assert.Equal(
            "My personal portfolio website",
            project.Description);

        Assert.Equal(
            "https://github.com/example/personal-site",
            project.RepositoryUrl);

        Assert.Equal(
            "https://example.com/",
            project.LiveUrl);

        Assert.True(project.IsFeatured);
        Assert.Equal(1, project.DisplayOrder);

        Assert.Equal(
            ContentSource.Official,
            project.Source);

        Assert.NotNull(project.Created);
        Assert.NotNull(project.Edited);
    }

    [Fact]
    public async Task PostProjectWithoutLiveUrlCreatesProject()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var tagId = SeedTag();

        var request = new CreateProjectRequest
        {
            Title = "API Project",
            Description = "A backend-only API project",
            RepositoryUrl = "https://github.com/example/api-project",
            LiveUrl = null,
            IsFeatured = false,
            TagIds = [tagId]
        };

        var response = await Client.PostAsJsonAsync(
            "/projects",
            request);

        var created = await response.ReadJsonAs<CreateProjectResponse>(
            HttpStatusCode.Created);

        Assert.Null(created.LiveUrl);

        var project = Reader.Query(
            context => context.Find<Project>(created.Id));

        Assert.NotNull(project);
        Assert.Null(project.LiveUrl);
    }

    [Fact]
    public async Task PostProjectWithWhitespaceLiveUrlStoresNull()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var tagId = SeedTag();

        var request = new CreateProjectRequest
        {
            Title = "Console Project",
            Description = "A console application",
            RepositoryUrl = "https://github.com/example/console-project",
            LiveUrl = "   ",
            IsFeatured = false,
            TagIds = [tagId]
        };

        var response = await Client.PostAsJsonAsync(
            "/projects",
            request);

        var created = await response.ReadJsonAs<CreateProjectResponse>(
            HttpStatusCode.Created);

        Assert.Null(created.LiveUrl);

        var project = Reader.Query(
            context => context.Find<Project>(created.Id));

        Assert.NotNull(project);
        Assert.Null(project.LiveUrl);
    }

    [Fact]
    public async Task PostProjectWithWhitespaceTitleReturnsBadRequest()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var tagId = SeedTag();

        var request = new CreateProjectRequest
        {
            Title = "   ",
            Description = "A valid project description",
            RepositoryUrl = "https://github.com/example/project",
            LiveUrl = null,
            IsFeatured = false,
            TagIds = [tagId]
        };

        var response = await Client.PostAsJsonAsync(
            "/projects",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task PostProjectWithWhitespaceDescriptionReturnsBadRequest()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var tagId = SeedTag();

        var request = new CreateProjectRequest
        {
            Title = "Valid Project",
            Description = "   ",
            RepositoryUrl = "https://github.com/example/project",
            LiveUrl = null,
            IsFeatured = false,
            TagIds = [tagId]
        };

        var response = await Client.PostAsJsonAsync(
            "/projects",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task PostProjectWithInvalidRepositoryUrlReturnsBadRequest()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var tagId = SeedTag();

        var request = new CreateProjectRequest
        {
            Title = "Valid Project",
            Description = "A valid project description",
            RepositoryUrl = "not-a-url",
            LiveUrl = null,
            IsFeatured = false,
            TagIds = [tagId]
        };

        var response = await Client.PostAsJsonAsync(
            "/projects",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task PostProjectWithInvalidLiveUrlReturnsBadRequest()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var tagId = SeedTag();

        var request = new CreateProjectRequest
        {
            Title = "Valid Project",
            Description = "A valid project description",
            RepositoryUrl = "https://github.com/example/project",
            LiveUrl = "invalid-live-url",
            IsFeatured = false,
            TagIds = [tagId]
        };

        var response = await Client.PostAsJsonAsync(
            "/projects",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private int SeedTag(string name = "C#")
    {
        var actor = new Actor(
            1,
            UserRole.Administrator);

        var tag = Tag.Create(
            actor,
            new TagName(name));

        Writer.Seed(
            context => context.Tags.Add(tag));

        return tag.Id;
    }
}