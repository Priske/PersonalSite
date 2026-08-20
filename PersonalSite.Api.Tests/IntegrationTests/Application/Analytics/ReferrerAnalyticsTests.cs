using System.Net;
using System.Net.Http.Json;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Application.Analytics.GetReferrerActivity;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Analytics;

public sealed class ReferrerAnalyticsTests : IntegrationTest
{
    [Fact]
    public async Task ReferrerAnalyticsRequiresAuthentication()
    {
        var response = await Client.GetAsync(
            "/analytics/referrers");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ReferrerAnalyticsRequiresAdministrator()
    {
        await AuthenticateAsUser(UserRole.User);

        var response = await Client.GetAsync(
            "/analytics/referrers");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReferrerAnalyticsGroupsByNormalizedHostAndDirectTraffic()
    {
        await TrackPageView("");
        await TrackPageView("https://www.google.com/search?q=portfolio");
        await TrackPageView("https://www.google.com/");
        await TrackPageView("https://github.com/example/repository");

        await AuthenticateAsUser(UserRole.Administrator);

        var response = await Client.GetAsync(
            "/analytics/referrers?sortBy=count&descending=true");

        var analytics = await response.ReadJsonAs<ReferrerAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(4, analytics.TotalPageViews);

        Assert.Contains(
            analytics.Referrers,
            item => item.Referrer == "Direct" && item.Count == 1);

        Assert.Contains(
            analytics.Referrers,
            item => item.Referrer == "www.google.com" && item.Count == 2);

        Assert.Contains(
            analytics.Referrers,
            item => item.Referrer == "github.com" && item.Count == 1);
    }

    [Fact]
    public async Task ReferrerAnalyticsCanSearchReferrer()
    {
        await TrackPageView("https://www.google.com/");
        await TrackPageView("https://github.com/example/repository");

        await AuthenticateAsUser(UserRole.Administrator);

        var response = await Client.GetAsync(
            "/analytics/referrers?search=github");

        var analytics = await response.ReadJsonAs<ReferrerAnalyticsResponse>(
            HttpStatusCode.OK);

        var item = Assert.Single(analytics.Referrers);

        Assert.Equal("github.com", item.Referrer);
        Assert.Equal(1, item.Count);
    }

    [Fact]
    public async Task ReferrerAnalyticsSortsByReferrerAscending()
    {
        await TrackPageView("https://zulu.example.com/");
        await TrackPageView("https://alpha.example.com/");

        await AuthenticateAsUser(UserRole.Administrator);

        var response = await Client.GetAsync(
            "/analytics/referrers?sortBy=referrer&descending=false");

        var analytics = await response.ReadJsonAs<ReferrerAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(
            ["alpha.example.com", "zulu.example.com"],
            analytics.Referrers.Select(item => item.Referrer).ToArray());
    }

    private async Task TrackPageView(string referrer)
    {
        var response = await Client.PostAsJsonAsync(
            "/analytics",
            new
            {
                type = ActivityType.PageViewed,
                metadata = new[]
                {
                    new
                    {
                        key = "Page",
                        value = new
                        {
                            path = "homepage",
                            referrer
                        }
                    }
                }
            });

        await response.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);
    }
}
