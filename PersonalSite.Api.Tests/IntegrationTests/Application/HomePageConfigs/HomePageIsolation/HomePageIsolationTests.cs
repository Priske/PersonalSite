using System.Net;
using System.Net.Http.Json;

using PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;
using PersonalSite.Api.Application.HomePageConfigs.UpdateConfig;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.HomePageConfigs
    .HomePageIsolation;

public sealed class HomePageIsolationTests : IntegrationTest
{
    [Fact]
    public async Task DifferentUsersReceiveDifferentDemoConfigurations()
    {
        var firstUserId =
            await AuthenticateAsUser(
                email:
                    "first@example.com");

        var firstResponse =
            await Client.GetAsync(
                "/home-demo-page-config");

        var first =
            await firstResponse.ReadJsonAs<GetHomePageConfigDetailsResponse>(
                HttpStatusCode.OK);

        var secondUserId =
            await AuthenticateAsUser(
                email:
                    "second@example.com");

        var secondResponse =
            await Client.GetAsync(
                "/home-demo-page-config");

        var second =
            await secondResponse.ReadJsonAs<GetHomePageConfigDetailsResponse>(
                HttpStatusCode.OK);

        Assert.NotEqual(
            firstUserId,
            secondUserId);

        Assert.Equal(
            firstUserId,
            first.CreatedByUserId);

        Assert.Equal(
            secondUserId,
            second.CreatedByUserId);
    }

    [Fact]
    public async Task UpdatingFirstUsersDemoDoesNotChangeSecondUsersDemo()
    {
        var firstUserId =
            await AuthenticateAsUser(
                email:
                    "first@example.com");

        await Client.GetAsync(
            "/home-demo-page-config");

        var firstUpdate =
            ValidRequest(
                heroFirstName:
                    "FirstUser");

        var firstUpdateResponse =
            await Client.PutAsJsonAsync(
                "/home-demo-page-config",
                firstUpdate);

        Assert.Equal(
            HttpStatusCode.NoContent,
            firstUpdateResponse.StatusCode);

        var secondUserId =
            await AuthenticateAsUser(
                email:
                    "second@example.com");

        var secondResponse =
            await Client.GetAsync(
                "/home-demo-page-config");

        var second =
            await secondResponse.ReadJsonAs<GetHomePageConfigDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            "First",
            second.HeroFirstName);

        var firstConfig =
            Reader.Query(
                db =>
                    db.HomepageConfigs.Single(
                        config =>
                            config.Source ==
                                ContentSource.Demo &&
                            config.Created.UserId ==
                                firstUserId));

        var secondConfig =
            Reader.Query(
                db =>
                    db.HomepageConfigs.Single(
                        config =>
                            config.Source ==
                                ContentSource.Demo &&
                            config.Created.UserId ==
                                secondUserId));

        Assert.Equal(
            "FirstUser",
            firstConfig.HeroFirstName.Value);

        Assert.Equal(
            "First",
            secondConfig.HeroFirstName.Value);
    }

    [Fact]
    public async Task RepeatedGetReturnsSameUsersDemoConfiguration()
    {
        var userId =
            await AuthenticateAsUser();

        await Client.GetAsync(
            "/home-demo-page-config");

        await Client.GetAsync(
            "/home-demo-page-config");

        var count =
            Reader.Query(
                db =>
                    db.HomepageConfigs.Count(
                        config =>
                            config.Source ==
                                ContentSource.Demo &&
                            config.Created.UserId ==
                                userId));

        Assert.Equal(
            1,
            count);
    }

    private static UpdateHomePageConfigRequest ValidRequest(
        string heroFirstName =
            "Ada")
    {
        return new UpdateHomePageConfigRequest
        {
            HeroBanner =
                "Updated banner",

            HeroFirstName =
                heroFirstName,

            HeroLastName =
                "Lovelace",

            HeroRole =
                "Software Developer",

            HeroEyebrow =
                "Hello",

            HeroHeading =
                "I build software",

            HeroSummary =
                "Updated summary",

            HeroPrimaryActionLabel =
                "Projects",

            HeroSecondaryActionLabel =
                "Contact",

            ContactSectionNumber =
                "02",

            ContactSectionEyebrow =
                "Contact",

            ContactSectionHeading =
                "Let's talk",

            ContactEyebrow =
                "Get in touch",

            ContactHeading =
                "Interested in working together?",

            ContactDescription =
                "Feel free to get in touch.",

            ContactEmailActionLabel =
                "Send email",

            ContactLoginActionLabel =
                "Login",

            Email =
                "demo@example.com",

            PhoneNumber =
                null,

            LinkedInUrl =
                null,

            GitHubUrl =
                null,

            CvUrl =
                null
        };
    }
}