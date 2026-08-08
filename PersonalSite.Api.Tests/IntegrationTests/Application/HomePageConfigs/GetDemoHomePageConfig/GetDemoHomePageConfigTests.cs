using System.Net;

using PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.HomePageConfigs
    .GetDemoHomePageConfig;

public sealed class GetDemoHomePageConfigTests : IntegrationTest
{
    [Fact]
    public async Task GetDemoHomePageConfigCreatesDemoForUser()
    {
        var userId =
            await AuthenticateAsUser();

        var response =
            await Client.GetAsync(
                "/home-demo-page-config");

        var result =
            await response.ReadJsonAs<GetHomePageConfigDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            "Welcome to my portfolio",
            result.HeroBanner);

        Assert.Equal(
            "First",
            result.HeroFirstName);

        Assert.Equal(
            "Last",
            result.HeroLastName);

        Assert.Equal(
            "Demo",
            result.Source);

        Assert.Equal(
            userId,
            result.CreatedByUserId);

        Assert.Equal(
            userId,
            result.LastEditedByUserId);

        var configs =
            Reader.Query(
                db =>
                    db.HomepageConfigs
                        .Where(
                            config =>
                                config.Source ==
                                ContentSource.Demo)
                        .ToList());

        var config =
            Assert.Single(configs);

        Assert.Equal(
            userId,
            config.Created.UserId);
    }

    [Fact]
    public async Task GetDemoHomePageConfigDoesNotCreateDuplicate()
    {
        var userId =
            await AuthenticateAsUser();

        await Client.GetAsync(
            "/home-demo-page-config");

        await Client.GetAsync(
            "/home-demo-page-config");

        var configs =
            Reader.Query(
                db =>
                    db.HomepageConfigs
                        .Where(
                            config =>
                                config.Source ==
                                    ContentSource.Demo &&
                                config.Created.UserId ==
                                    userId)
                        .ToList());

        Assert.Single(configs);
    }

    [Fact]
    public async Task DifferentUsersGetDifferentDemoConfigs()
    {
        var firstUserId =
            await AuthenticateAsUser(
                email: "first@example.com");

        await Client.GetAsync(
            "/home-demo-page-config");

        var secondUserId =
            await AuthenticateAsUser(
                email: "second@example.com");

        await Client.GetAsync(
            "/home-demo-page-config");

        var configs =
            Reader.Query(
                db =>
                    db.HomepageConfigs
                        .Where(
                            config =>
                                config.Source ==
                                ContentSource.Demo)
                        .ToList());

        Assert.Equal(
            2,
            configs.Count);

        Assert.Contains(
            configs,
            config =>
                config.Created.UserId ==
                firstUserId);

        Assert.Contains(
            configs,
            config =>
                config.Created.UserId ==
                secondUserId);
    }

    [Fact]
    public async Task GetDemoHomePageConfigWithoutAuthenticationReturnsUnauthorized()
    {
        var response =
            await Client.GetAsync(
                "/home-demo-page-config");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}