using System.Net;
using System.Net.Http.Json;
using PersonalSite.Api.Application.Projects.GetProjectSummeries;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application
    .Projects.GetProjectSummariesTests;

public class GetProjectSummariesTests : IntegrationTest
{
    [Fact]
    public async Task GetProjectsReturnsProjects()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        SeedProject(
            title: "Personal Site",
            description: "My portfolio website",
            repositoryUrl: "https://github.com/example/personal-site",
            liveUrl: "https://example.com",
            displayOrder: 1);

        SeedProject(
            title: "Book Tracker",
            description: "An application for tracking books",
            repositoryUrl: "https://github.com/example/book-tracker",
            liveUrl: null,
            displayOrder: 2);

        var response =
            await Client.GetAsync("/projects");

        var content =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Equal(2, content.TotalItems);
        Assert.Equal(1, content.Page);
        Assert.Equal(10, content.PageSize);
        Assert.Equal(1, content.TotalPages);
        Assert.Equal(2, content.Items.Count);

        var first = content.Items[0];

        Assert.Equal("Personal Site", first.Title);
        Assert.Equal("My portfolio website", first.Discription);
        Assert.Equal(
            "https://github.com/example/personal-site",
            first.RepositoryUrl);
        Assert.Equal("https://example.com/", first.LiveUrl);
        Assert.True(first.IsFeatured);
        Assert.Equal(1, first.DisplayOrder);

        var second = content.Items[1];

        Assert.Equal("Book Tracker", second.Title);
        Assert.Null(second.LiveUrl);
    }

    [Fact]
    public async Task GetProjectsReturnsEmptyCollectionWhenNoProjectsExist()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var response =
            await Client.GetAsync("/projects");

        var content =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Empty(content.Items);
        Assert.Equal(0, content.TotalItems);
        Assert.Equal(0, content.TotalPages);
        Assert.Equal(1, content.Page);
        Assert.Equal(10, content.PageSize);
    }

    [Fact]
    public async Task GetProjectsUsesRequestedPageAndPageSize()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        for (var index = 1; index <= 5; index++)
        {
            SeedProject(
                title: $"Project {index}",
                description: $"Description {index}",
                repositoryUrl:
                    $"https://github.com/example/project-{index}",
                liveUrl: null,
                displayOrder: index);
        }

        var response =
            await Client.GetAsync(
                "/projects?page=2&pageSize=2");

        var content =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Equal(5, content.TotalItems);
        Assert.Equal(3, content.TotalPages);
        Assert.Equal(2, content.Page);
        Assert.Equal(2, content.PageSize);
        Assert.Equal(2, content.Items.Count);

        Assert.Equal("Project 3", content.Items[0].Title);
        Assert.Equal("Project 4", content.Items[1].Title);
    }

    [Fact]
    public async Task GetProjectsSearchesByTitle()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        SeedProject(
            title: "Personal Site",
            description: "Portfolio",
            repositoryUrl: "https://github.com/example/personal-site",
            liveUrl: null,
            displayOrder: 1);

        SeedProject(
            title: "Book Tracker",
            description: "Books",
            repositoryUrl: "https://github.com/example/book-tracker",
            liveUrl: null,
            displayOrder: 2);

        var response =
            await Client.GetAsync("/projects?search=personal");

        var content =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Single(content.Items);
        Assert.Equal("Personal Site", content.Items[0].Title);
        Assert.Equal(1, content.TotalItems);
    }

    [Fact]
    public async Task GetProjectsSearchesByDescription()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        SeedProject(
            title: "Personal Site",
            description: "Portfolio built with ASP.NET Core",
            repositoryUrl: "https://github.com/example/personal-site",
            liveUrl: null,
            displayOrder: 1);

        SeedProject(
            title: "Book Tracker",
            description: "Application for books",
            repositoryUrl: "https://github.com/example/book-tracker",
            liveUrl: null,
            displayOrder: 2);

        var response =
            await Client.GetAsync("/projects?search=ASP.NET");

        var content =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Single(content.Items);
        Assert.Equal("Personal Site", content.Items[0].Title);
    }

    [Fact]
    public async Task GetProjectsSearchesByRepositoryUrl()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        SeedProject(
            title: "Personal Site",
            description: "Portfolio",
            repositoryUrl:
                "https://github.com/example/personal-site",
            liveUrl: null,
            displayOrder: 1);

        SeedProject(
            title: "Book Tracker",
            description: "Books",
            repositoryUrl:
                "https://gitlab.com/example/book-tracker",
            liveUrl: null,
            displayOrder: 2);

        var response =
            await Client.GetAsync("/projects?search=gitlab");

        var content =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Single(content.Items);
        Assert.Equal("Book Tracker", content.Items[0].Title);
    }

    [Fact]
    public async Task GetProjectsClampsInvalidPaginationValues()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var response =
            await Client.GetAsync(
                "/projects?page=-5&pageSize=1000");

        var content =
            await response.ReadJsonAs<GetProjectSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Equal(1, content.Page);
        Assert.Equal(50, content.PageSize);
    }

    private Project SeedProject(
        string title,
        string description,
        string repositoryUrl,
        string? liveUrl,
        int displayOrder)
    {
        var project = new Project
        {
            Title = new ProjectTitle(title),
            Description = new ProjectDiscription(description),
            RepositoryUrl = new Url(repositoryUrl),
            LiveUrl = liveUrl is null
                ? null
                : new Url(liveUrl),
            IsFeatured = displayOrder == 1,
            DisplayOrder = displayOrder
        };

        Writer.Seed(db => db.Projects.Add(project));

        return project;
    }
}