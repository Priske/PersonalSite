using System.Net;
using System.Net.Http.Json;
using PersonalSite.Api.Application.Analytics.GetLoginActivity;
using PersonalSite.Api.Application.Auth.Login;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Analytics;

public sealed class LoginAnalyticsTests : IntegrationTest
{
    [Fact]
    public async Task LoginAnalyticsRequiresAuthentication()
    {
        var response = await Client.GetAsync("/analytics/login");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LoginAnalyticsReturnsLoginSummaryAndFailureReasons()
    {
        var userId = await AuthenticateAsUser(
            UserRole.Administrator);

        var wrongPasswordResponse = await Client.PostAsJsonAsync(
            "/auth/login",
            new LoginRequest
            {
                Email = "ada@example.com",
                Password = "definitely-wrong-password"
            });

        await wrongPasswordResponse.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);

        var unknownEmailResponse = await Client.PostAsJsonAsync(
            "/auth/login",
            new LoginRequest
            {
                Email = "nobody@example.com",
                Password = "does-not-matter"
            });

        await unknownEmailResponse.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);

        var response = await Client.GetAsync(
            "/analytics/login?page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<LoginAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(3, analytics.Summary.TotalAttempts);
        Assert.Equal(1, analytics.Summary.SuccessfulLogins);
        Assert.Equal(2, analytics.Summary.FailedLogins);
        Assert.Equal(1, analytics.Summary.UnknownEmailAttempts);
        Assert.Equal(1, analytics.Summary.IncorrectPasswordAttempts);

        Assert.Contains(
            analytics.Items,
            item =>
                item.UserId == userId &&
                item.Successful &&
                item.FailureReason is null);

        Assert.Contains(
            analytics.Items,
            item =>
                item.UserId == userId &&
                !item.Successful &&
                item.FailureReason == "incorrect_password");

        Assert.Contains(
            analytics.Items,
            item =>
                item.UserId is null &&
                !item.Successful &&
                item.FailureReason == "unknown_email");
    }

    [Fact]
    public async Task LoginAnalyticsCanFilterFailedLogins()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        await Client.PostAsJsonAsync(
            "/auth/login",
            new LoginRequest
            {
                Email = "nobody@example.com",
                Password = "does-not-matter"
            });

        var response = await Client.GetAsync(
            "/analytics/login?successful=false&page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<LoginAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(1, analytics.TotalItems);
        Assert.All(
            analytics.Items,
            item => Assert.False(item.Successful));
    }

    [Fact]
    public async Task LoginAnalyticsCanSearchByFailureReason()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        await Client.PostAsJsonAsync(
            "/auth/login",
            new LoginRequest
            {
                Email = "nobody@example.com",
                Password = "does-not-matter"
            });

        var response = await Client.GetAsync(
            "/analytics/login?search=unknown&page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<LoginAnalyticsResponse>(
            HttpStatusCode.OK);

        var item = Assert.Single(analytics.Items);

        Assert.False(item.Successful);
        Assert.Equal("unknown_email", item.FailureReason);
    }

    [Fact]
    public async Task LoginAnalyticsPaginates()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        for (var i = 0; i < 4; i++)
        {
            await Client.PostAsJsonAsync(
                "/auth/login",
                new LoginRequest
                {
                    Email = $"unknown-{i}@example.com",
                    Password = "does-not-matter"
                });
        }

        var response = await Client.GetAsync(
            "/analytics/login?page=2&pageSize=2");

        var analytics = await response.ReadJsonAs<LoginAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(2, analytics.Page);
        Assert.Equal(2, analytics.PageSize);
        Assert.Equal(5, analytics.TotalItems);
        Assert.Equal(3, analytics.TotalPages);
        Assert.Equal(2, analytics.Items.Count);
    }
}
