using System.Net;
using System.Net.Http.Json;

using PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;
using PersonalSite.Api.Application.HomePageConfigs.UpdateConfig;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.HomePageConfigs
    .UpdateOfficialHomePageConfig;

public sealed class UpdateOfficialHomePageConfigTests : IntegrationTest
{
    [Fact]
    public async Task AdministratorCanUpdateOfficialHomePageConfig()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var request =
            ValidRequest();

        var response =
            await Client.PutAsJsonAsync(
                "/home-official-page-config",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var getResponse =
            await Client.GetAsync(
                "/home-official-page-config");

        var result =
            await getResponse.ReadJsonAs<GetHomePageConfigDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            "Updated banner",
            result.HeroBanner);

        Assert.Equal(
            "Ada",
            result.HeroFirstName);

        Assert.Equal(
            "Lovelace",
            result.HeroLastName);

        Assert.Equal(
            "Backend Developer",
            result.HeroRole);

        Assert.Equal(
            "Updated heading",
            result.HeroHeading);

        Assert.Equal(
            "updated@example.com",
            result.Email);

        Assert.Equal(
            "+32485123456",
            result.PhoneNumber);

        Assert.Equal(
            "https://linkedin.com/in/example",
            result.LinkedInUrl);

        Assert.Equal(
            "Official",
            result.Source);
    }

    [Fact]
    public async Task RegularUserCannotUpdateOfficialHomePageConfig()
    {
        await AuthenticateAsUser();

        var response =
            await Client.PutAsJsonAsync(
                "/home-official-page-config",
                ValidRequest());

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateOfficialHomePageConfigWithInvalidEmailReturnsBadRequest()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var request =
            ValidRequest(
                email: "not-an-email");

        var response =
            await Client.PutAsJsonAsync(
                "/home-official-page-config",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateOfficialHomePageConfigWithInvalidSectionNumberReturnsBadRequest()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var request =
            ValidRequest(
                contactSectionNumber: "ABC");

        var response =
            await Client.PutAsJsonAsync(
                "/home-official-page-config",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateOfficialHomePageConfigWithWhitespaceOptionalValuesStoresNull()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var request =
            ValidRequest(
                phoneNumber: "   ",
                linkedInUrl: "   ",
                gitHubUrl: "   ",
                cvUrl: "   ");

        var response =
            await Client.PutAsJsonAsync(
                "/home-official-page-config",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var result =
            Reader.Query(
                db =>
                    db.HomepageConfigs.Single(
                        config =>
                            config.Source ==
                            ContentSource.Official));

        Assert.Null(
            result.PhoneNumber);

        Assert.Null(
            result.LinkedInUrl);

        Assert.Null(
            result.GitHubUrl);

        Assert.Null(
            result.CvUrl);
    }

    private static UpdateHomePageConfigRequest ValidRequest(
        string email = "updated@example.com",
        string contactSectionNumber = "03",
        string? phoneNumber = "+32 485 12 34 56",
        string? linkedInUrl = "https://linkedin.com/in/example",
        string? gitHubUrl = "https://github.com/example",
        string? cvUrl = "https://example.com/cv.pdf")
    {
        return new UpdateHomePageConfigRequest
        {
            HeroBanner = "Updated banner",
            HeroFirstName = "Ada",
            HeroLastName = "Lovelace",
            HeroRole = "Backend Developer",

            HeroEyebrow = "Hello there",
            HeroHeading = "Updated heading",

            HeroSummary =
                "I create reliable software applications.",

            HeroPrimaryActionLabel =
                "View projects",

            HeroSecondaryActionLabel =
                "Contact me",

            ContactSectionNumber =
                contactSectionNumber,

            ContactSectionEyebrow =
                "Get in touch",

            ContactSectionHeading =
                "Contact",

            ContactEyebrow =
                "Have an opportunity?",

            ContactHeading =
                "Let's talk",

            ContactDescription =
                "Feel free to contact me about software development opportunities.",

            ContactEmailActionLabel =
                "Send email",

            ContactLoginActionLabel =
                "Login",

            Email = email,

            PhoneNumber = phoneNumber,
            LinkedInUrl = linkedInUrl,
            GitHubUrl = gitHubUrl,
            CvUrl = cvUrl
        };
    }
}