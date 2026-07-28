using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed class HomePageConfig
{
    public int Id { get; private set; }

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
}