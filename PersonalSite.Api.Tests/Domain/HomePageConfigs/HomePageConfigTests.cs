using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class HomePageConfigTests
{
    [Fact]
    public void CreateDemo_CreatesDemoHomePageConfig()
    {
        const int userId =
            42;

        var config =
            HomePageConfig.CreateDemo(
                userId);

        Assert.Equal(
            ContentSource.Demo,
            config.Source);

        Assert.Equal(
            userId,
            config.Created.UserId);

        Assert.Equal(
            userId,
            config.Edited.UserId);

        Assert.Equal(
            config.Created.At,
            config.Edited.At);
    }

    [Fact]
    public void CreateDemo_CreatesDefaultHeroContent()
    {
        var config =
            HomePageConfig.CreateDemo(
                42);

        Assert.Equal(
            "Welcome to my portfolio",
            config.HeroBanner.Value);

        Assert.Equal(
            "First",
            config.HeroFirstName.Value);

        Assert.Equal(
            "Last",
            config.HeroLastName.Value);

        Assert.Equal(
            "Software Developer",
            config.HeroRole.Value);

        Assert.Equal(
            "Hello",
            config.HeroEyebrow.Value);

        Assert.Equal(
            "I build reliable software",
            config.HeroHeading.Value);

        Assert.Equal(
            "This is your demo homepage. Edit the content to preview your own version.",
            config.HeroSummary.Value);

        Assert.Equal(
            "View projects",
            config.HeroPrimaryActionLabel.Value);

        Assert.Equal(
            "Contact me",
            config.HeroSecondaryActionLabel.Value);
    }

    [Fact]
    public void CreateDemo_CreatesDefaultContactContent()
    {
        var config =
            HomePageConfig.CreateDemo(
                42);

        Assert.Equal(
            "02",
            config.ContactSectionNumber.Value);

        Assert.Equal(
            "Contact",
            config.ContactSectionEyebrow.Value);

        Assert.Equal(
            "Let's talk",
            config.ContactSectionHeading.Value);

        Assert.Equal(
            "Get in touch",
            config.ContactEyebrow.Value);

        Assert.Equal(
            "Interested in working together?",
            config.ContactHeading.Value);

        Assert.Equal(
            "Feel free to get in touch.",
            config.ContactDescription.Value);

        Assert.Equal(
            "Send email",
            config.ContactEmailActionLabel.Value);

        Assert.Equal(
            "Login",
            config.ContactLoginActionLabel.Value);

        Assert.Equal(
            "demo@example.com",
            config.Email.Value);
    }

    [Fact]
    public void CreateDemo_OptionalContactValuesAreNull()
    {
        var config =
            HomePageConfig.CreateDemo(
                42);

        Assert.Null(
            config.PhoneNumber);

        Assert.Null(
            config.LinkedInUrl);

        Assert.Null(
            config.GitHubUrl);

        Assert.Null(
            config.CvUrl);
    }
}