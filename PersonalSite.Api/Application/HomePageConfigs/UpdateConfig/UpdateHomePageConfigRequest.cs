namespace PersonalSite.Api.Application.HomePageConfigs.UpdateConfig;

public class UpdateHomePageConfigRequest
{
    public required string HeroEyebrow { get; set; }
    public required string HeroHeading { get; set; }
    public required string HeroSummary { get; set; }

    public required string ContactHeading { get; set; }
    public required string ContactDescription { get; set; }

    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? CvUrl { get; set; }


}