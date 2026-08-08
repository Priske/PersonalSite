using System.Net;

using PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.HomePageConfigs
    .GetOfficialHomePageConfig;

public sealed class GetOfficialHomePageConfigTests : IntegrationTest
{
    [Fact]
    public async Task GetOfficialHomePageConfigReturnsConfig()
    {
        var response =
            await Client.GetAsync(
                "/home-official-page-config");

        var result =
            await response.ReadJsonAs<GetHomePageConfigDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            "Software developer",
            result.HeroBanner);

        Assert.Equal(
            "Ben",
            result.HeroFirstName);

        Assert.Equal(
            "Eeckman",
            result.HeroLastName);

        Assert.Equal(
            "Junior software developer",
            result.HeroRole);

        Assert.Equal(
            "Official",
            result.Source);

        Assert.NotNull(
            result.CreatedByUserId);

        Assert.NotNull(
            result.LastEditedByUserId);
    }

    [Fact]
    public async Task OfficialHomePageConfigDoesNotRequireAuthentication()
    {
        var response =
            await Client.GetAsync(
                "/home-official-page-config");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}