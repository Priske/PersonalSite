using System.Net;
using System.Net.Http.Json;
using PersonalSite.Api.Application.Analytics.GetCreateUserRequest;
using PersonalSite.Api.Application.Users.CreateUsers;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Analytics;

public sealed class CreateUserAnalyticsTests : IntegrationTest
{
    [Fact]
    public async Task CreateUserAnalyticsReturnsCreatedUsers()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var ada = await CreateUser(
            "Ada Analytics",
            "ada.analytics@example.com");

        var grace = await CreateUser(
            "Grace Analytics",
            "grace.analytics@example.com");

        var response = await Client.GetAsync(
            "/analytics/create-users?page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<CreateUserAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(2, analytics.Summary.TotalCreatedUsers);
        Assert.Equal(2, analytics.TotalItems);

        Assert.Contains(
            analytics.Items,
            item =>
                item.UserId == ada.Id &&
                item.Name == "Ada Analytics" &&
                item.Email == "ada.analytics@example.com");

        Assert.Contains(
            analytics.Items,
            item =>
                item.UserId == grace.Id &&
                item.Name == "Grace Analytics" &&
                item.Email == "grace.analytics@example.com");
    }

    [Fact]
    public async Task CreateUserAnalyticsCanSearchByNameOrEmail()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        await CreateUser(
            "Ada Analytics",
            "ada.analytics@example.com");

        await CreateUser(
            "Grace Analytics",
            "grace.analytics@example.com");

        var response = await Client.GetAsync(
            "/analytics/create-users?search=grace&page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<CreateUserAnalyticsResponse>(
            HttpStatusCode.OK);

        var item = Assert.Single(analytics.Items);

        Assert.Equal("Grace Analytics", item.Name);
        Assert.Equal("grace.analytics@example.com", item.Email);
    }

    [Fact]
    public async Task CreateUserAnalyticsSortsByNameAscending()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        await CreateUser(
            "Zulu User",
            "zulu@example.com");

        await CreateUser(
            "Alpha User",
            "alpha@example.com");

        var response = await Client.GetAsync(
            "/analytics/create-users?sortBy=name&descending=false&page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<CreateUserAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(
            ["Alpha User", "Zulu User"],
            analytics.Items.Select(item => item.Name).ToArray());
    }

    [Fact]
    public async Task CreateUserAnalyticsPaginates()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        await CreateUser("User One", "one@example.com");
        await CreateUser("User Two", "two@example.com");
        await CreateUser("User Three", "three@example.com");

        var response = await Client.GetAsync(
            "/analytics/create-users?page=2&pageSize=2");

        var analytics = await response.ReadJsonAs<CreateUserAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(2, analytics.Page);
        Assert.Equal(2, analytics.PageSize);
        Assert.Equal(3, analytics.TotalItems);
        Assert.Equal(2, analytics.TotalPages);
        Assert.Single(analytics.Items);
    }

    private async Task<CreateUserResponse> CreateUser(
        string name,
        string email)
    {
        var response = await Client.PostAsJsonAsync(
            "/users",
            new CreateUserRequest
            {
                Name = name,
                Email = email,
                Password = "analytics-test-password"
            });

        return await response.ReadJsonAs<CreateUserResponse>(
            HttpStatusCode.Created);
    }
}
