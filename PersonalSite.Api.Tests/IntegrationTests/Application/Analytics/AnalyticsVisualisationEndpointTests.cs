using System.Net;
using System.Text.Json;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Analytics;

public sealed class AnalyticsVisualisationEndpointTests : IntegrationTest
{
    [Fact]
    public async Task Every_dashboard_endpoint_accepts_named_zones_and_returns_validated_daily_contracts()
    {
        await AuthenticateAsUser(UserRole.Administrator);
        string[] endpoints = ["login", "referrers", "contact-links", "redirect-links", "videos", "create-users", "delete-users"];

        foreach (var endpoint in endpoints)
        {
            var path = $"/analytics/{endpoint}";
            using var response = await Client.GetAsync($"{path}?timeZone=Europe%2FBrussels");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal(JsonValueKind.Array, body.RootElement.GetProperty("daily").ValueKind);
            if (endpoint == "referrers")
            {
                Assert.Equal(JsonValueKind.Number, body.RootElement.GetProperty("matchingPageViews").ValueKind);
                Assert.Equal(JsonValueKind.Array, body.RootElement.GetProperty("dailyByReferrer").ValueKind);
            }
            if (endpoint is "contact-links" or "redirect-links")
                Assert.Equal(JsonValueKind.Array, body.RootElement.GetProperty("dailyByLink").ValueKind);
            if (endpoint == "videos")
                Assert.Equal(JsonValueKind.Array, body.RootElement.GetProperty("dailyByVideo").ValueKind);

            using var invalidZone = await Client.GetAsync($"{path}?timeZone=Not%2FAZone");
            Assert.Equal(HttpStatusCode.BadRequest, invalidZone.StatusCode);
            using var reversedDates = await Client.GetAsync($"{path}?from=2026-03-30T00%3A00%3A00Z&to=2026-03-29T00%3A00%3A00Z");
            Assert.Equal(HttpStatusCode.BadRequest, reversedDates.StatusCode);

            using var empty = await Client.GetAsync($"{path}?timeZone=Europe%2FBrussels&from=2099-01-01T00%3A00%3A00Z&to=2099-01-02T00%3A00%3A00Z");
            Assert.Equal(HttpStatusCode.OK, empty.StatusCode);
            using var emptyBody = JsonDocument.Parse(await empty.Content.ReadAsStringAsync());
            Assert.Empty(emptyBody.RootElement.GetProperty("daily").EnumerateArray());
        }
    }
}
