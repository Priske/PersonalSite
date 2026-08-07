namespace PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;

public sealed class GetHomePageConfigDetailsResponse
{
    public required string HeroBanner { get; init; }
    public required string HeroFirstName { get; init; }
    public required string HeroLastName { get; init; }
    public required string HeroRole { get; init; }

    public required string HeroEyebrow { get; init; }
    public required string HeroHeading { get; init; }
    public required string HeroSummary { get; init; }

    public required string HeroPrimaryActionLabel { get; init; }
    public required string HeroSecondaryActionLabel { get; init; }

    public required string ContactSectionNumber { get; init; }
    public required string ContactSectionEyebrow { get; init; }
    public required string ContactSectionHeading { get; init; }

    public required string ContactEyebrow { get; init; }
    public required string ContactHeading { get; init; }
    public required string ContactDescription { get; init; }

    public required string ContactEmailActionLabel { get; init; }
    public required string ContactLoginActionLabel { get; init; }

    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? LinkedInUrl { get; init; }
    public string? GitHubUrl { get; init; }
    public string? CvUrl { get; init; }

    public required string Source { get; init; }
    public int? CreatedByUserId { get; init; }
    public int? LastEditedByUserId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset LastEditedAt { get; init; }

}