using PersonalSite.Api.Domain.Common;


namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed class HomePageConfig
{
    public int Id { get; private set; }

    // Hero
    public required HeroEyebrow HeroEyebrow { get; set; }
    public required HeroHeading HeroHeading { get; set; }
    public required HeroSummary HeroSummary { get; set; }

    // Contact
    public required ContactHeading ContactHeading { get; set; }
    public required ContactDescription ContactDescription { get; set; }

    public required EmailAddress Email { get; set; }
    public PhoneNumber? PhoneNumber { get; set; }

    public Url? LinkedInUrl { get; set; }
    public Url? GitHubUrl { get; set; }
    public Url? CvUrl { get; set; }
}