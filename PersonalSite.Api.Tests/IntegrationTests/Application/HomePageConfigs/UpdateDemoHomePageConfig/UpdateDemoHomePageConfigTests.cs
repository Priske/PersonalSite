using System.Net;
using System.Net.Http.Json;

using PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;
using PersonalSite.Api.Application.HomePageConfigs.UpdateConfig;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.HomePageConfigs
    .UpdateDemoHomePageConfig;

public sealed class UpdateDemoHomePageConfigTests : IntegrationTest
{
    [Fact]
    public async Task RegularUserCanUpdateOwnDemoHomePageConfig()
    {
        var userId =
            await AuthenticateAsUser();

        await Client.GetAsync(
            "/home-demo-page-config");

        var response =
            await Client.PutAsJsonAsync(
                "/home-demo-page-config",
                ValidRequest());

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var getResponse =
            await Client.GetAsync(
                "/home-demo-page-config");

        var result =
            await getResponse.ReadJsonAs<GetHomePageConfigDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            "Updated demo banner",
            result.HeroBanner);

        Assert.Equal(
            "Demo",
            result.Source);

        Assert.Equal(
            userId,
            result.CreatedByUserId);

        Assert.Equal(
            userId,
            result.LastEditedByUserId);

        Assert.Equal(
            "demo.updated@example.com",
            result.Email);
    }

    [Fact]
    public async Task UpdateDemoHomePageConfigBeforeDemoExistsReturnsNotFound()
    {
        await AuthenticateAsUser();

        var response =
            await Client.PutAsJsonAsync(
                "/home-demo-page-config",
                ValidRequest());

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateDemoHomePageConfigWithInvalidEmailReturnsBadRequest()
    {
        await AuthenticateAsUser();

        await Client.GetAsync(
            "/home-demo-page-config");

        var response =
            await Client.PutAsJsonAsync(
                "/home-demo-page-config",
                ValidRequest(
                    email: "invalid-email"));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateDemoHomePageConfigWithInvalidSectionNumberReturnsBadRequest()
    {
        await AuthenticateAsUser();

        await Client.GetAsync(
            "/home-demo-page-config");

        var response =
            await Client.PutAsJsonAsync(
                "/home-demo-page-config",
                ValidRequest(
                    contactSectionNumber: "section-three"));

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateDemoHomePageConfigWithWhitespaceOptionalValuesStoresNull()
    {
        var userId =
            await AuthenticateAsUser();

        await Client.GetAsync(
            "/home-demo-page-config");

        var response =
            await Client.PutAsJsonAsync(
                "/home-demo-page-config",
                ValidRequest(
                    phoneNumber: "   ",
                    linkedInUrl: "   ",
                    gitHubUrl: "   ",
                    cvUrl: "   "));

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var config =
            Reader.Query(
                db =>
                    db.HomepageConfigs.Single(
                        current =>
                            current.Source ==
                                ContentSource.Demo &&
                            current.Created.UserId ==
                                userId));

        Assert.Null(
            config.PhoneNumber);

        Assert.Null(
            config.LinkedInUrl);

        Assert.Null(
            config.GitHubUrl);

        Assert.Null(
            config.CvUrl);
    }

    [Fact]
    public async Task UpdateDemoHomePageConfigUpdatesEditedMetadata()
    {
        var userId =
            await AuthenticateAsUser();

        await Client.GetAsync(
            "/home-demo-page-config");

        var before =
            Reader.Query(
                db =>
                    db.HomepageConfigs.Single(
                        current =>
                            current.Source ==
                                ContentSource.Demo &&
                            current.Created.UserId ==
                                userId));

        var previousEditedAt =
            before.Edited.At;

        await Task.Delay(10);

        var response =
            await Client.PutAsJsonAsync(
                "/home-demo-page-config",
                ValidRequest());

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updated =
            Reader.Query(
                db =>
                    db.HomepageConfigs.Single(
                        current =>
                            current.Source ==
                                ContentSource.Demo &&
                            current.Created.UserId ==
                                userId));

        Assert.Equal(
            userId,
            updated.Edited.UserId);

        Assert.True(
            updated.Edited.At >
            previousEditedAt);
    }

    private static UpdateHomePageConfigRequest ValidRequest(
        string email = "demo.updated@example.com",
        string contactSectionNumber = "03",
        string? phoneNumber = "+32 485 12 34 56",
        string? linkedInUrl = "https://linkedin.com/in/demo",
        string? gitHubUrl = "https://github.com/demo",
        string? cvUrl = "https://example.com/demo-cv.pdf")
    {
        return new UpdateHomePageConfigRequest
        {
            HeroBanner =
                "Updated demo banner",

            HeroFirstName =
                "Ada",

            HeroLastName =
                "Lovelace",

            HeroRole =
                "Software Developer",

            HeroEyebrow =
                "Hello",

            HeroHeading =
                "I build software",

            HeroSummary =
                "This is my updated demo homepage configuration.",

            HeroPrimaryActionLabel =
                "Projects",

            HeroSecondaryActionLabel =
                "Contact",

            ContactSectionNumber =
                contactSectionNumber,

            ContactSectionEyebrow =
                "Contact",

            ContactSectionHeading =
                "Get in touch",

            ContactEyebrow =
                "Have an opportunity?",

            ContactHeading =
                "Let's talk",

            ContactDescription =
                "Feel free to contact me about development opportunities.",

            ContactEmailActionLabel =
                "Email me",

            ContactLoginActionLabel =
                "Login",

            Email =
                email,

            PhoneNumber =
                phoneNumber,

            LinkedInUrl =
                linkedInUrl,

            GitHubUrl =
                gitHubUrl,

            CvUrl =
                cvUrl
        };
    }
}