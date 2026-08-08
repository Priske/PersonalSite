using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed class HomePageConfig : SiteContent
{
    public int Id { get; private set; }

    private HomePageConfig()
    {
    }

    internal HomePageConfig(
        ContentSource source,
        int userId)
    {
        var now = DateTimeOffset.UtcNow;

        Source = source;
        Created = new Change(userId, now);
        Edited = new Change(userId, now);
    }

    public required HomePageText HeroBanner { get; set; }
    public required HomePageText HeroFirstName { get; set; }
    public required HomePageText HeroLastName { get; set; }
    public required HomePageText HeroRole { get; set; }

    public required HeroEyebrow HeroEyebrow { get; set; }
    public required HeroHeading HeroHeading { get; set; }
    public required HeroSummary HeroSummary { get; set; }

    public required HomePageText HeroPrimaryActionLabel { get; set; }
    public required HomePageText HeroSecondaryActionLabel { get; set; }

    public required SectionNumber ContactSectionNumber { get; set; }
    public required HomePageText ContactSectionEyebrow { get; set; }
    public required HomePageText ContactSectionHeading { get; set; }

    public required HomePageText ContactEyebrow { get; set; }
    public required ContactHeading ContactHeading { get; set; }
    public required ContactDescription ContactDescription { get; set; }

    public required HomePageText ContactEmailActionLabel { get; set; }
    public required HomePageText ContactLoginActionLabel { get; set; }

    public required EmailAddress Email { get; set; }

    public PhoneNumber? PhoneNumber { get; set; }
    public Url? LinkedInUrl { get; set; }
    public Url? GitHubUrl { get; set; }
    public Url? CvUrl { get; set; }

    public static HomePageConfig CreateDemo(
        int userId)
    {
        return new HomePageConfig(
            ContentSource.Demo,
            userId)
        {
            HeroBanner = new HomePageText(
                "Welcome to my portfolio",
                "Hero Banner"),

            HeroFirstName = new HomePageText(
                "First",
                "Hero First Name"),

            HeroLastName = new HomePageText(
                "Last",
                "Hero Last Name"),

            HeroRole = new HomePageText(
                "Software Developer",
                "Hero Role"),

            HeroEyebrow = new HeroEyebrow(
                "Hello"),

            HeroHeading = new HeroHeading(
                "I build reliable software"),

            HeroSummary = new HeroSummary(
                "This is your demo homepage. Edit the content to preview your own version."),

            HeroPrimaryActionLabel = new HomePageText(
                "View projects",
                "Hero Primary Action Label"),

            HeroSecondaryActionLabel = new HomePageText(
                "Contact me",
                "Hero Secondary Action Label"),

            ContactSectionNumber =
                new SectionNumber("02"),

            ContactSectionEyebrow = new HomePageText(
                "Contact",
                "Contact Section Eyebrow"),

            ContactSectionHeading = new HomePageText(
                "Let's talk",
                "Contact Section Heading"),

            ContactEyebrow = new HomePageText(
                "Get in touch",
                "Contact Eyebrow"),

            ContactHeading = new ContactHeading(
                "Interested in working together?"),

            ContactDescription = new ContactDescription(
                "Feel free to get in touch."),

            ContactEmailActionLabel = new HomePageText(
                "Send email",
                "Contact Email Action Label"),

            ContactLoginActionLabel = new HomePageText(
                "Login",
                "Contact Login Action Label"),

            Email = new EmailAddress(
                "demo@example.com")
        };
    }
}