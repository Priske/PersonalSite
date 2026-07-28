using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;

public sealed class GetHomePageDetailsQueryHandler(AppDbContext dbContext) : IHandler
{
    public async Task<GetHomePageConfigDetailsResponse?> Execute(
        CancellationToken cancellationToken)
    {
        var config = await dbContext.HomepageConfigs
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (config is null)
        {
            return null;
        }

        return new GetHomePageConfigDetailsResponse
        {
            HeroBanner = config.HeroBanner,
            HeroFirstName = config.HeroFirstName,
            HeroLastName = config.HeroLastName,
            HeroRole = config.HeroRole,

            HeroEyebrow = config.HeroEyebrow,
            HeroHeading = config.HeroHeading,
            HeroSummary = config.HeroSummary,

            HeroPrimaryActionLabel = config.HeroPrimaryActionLabel,
            HeroSecondaryActionLabel = config.HeroSecondaryActionLabel,

            ContactSectionNumber = config.ContactSectionNumber,
            ContactSectionEyebrow = config.ContactSectionEyebrow,
            ContactSectionHeading = config.ContactSectionHeading,

            ContactEyebrow = config.ContactEyebrow,
            ContactHeading = config.ContactHeading,
            ContactDescription = config.ContactDescription,

            ContactEmailActionLabel = config.ContactEmailActionLabel,
            ContactLoginActionLabel = config.ContactLoginActionLabel,

            Email = config.Email,
            PhoneNumber = config.PhoneNumber?.Value,
            LinkedInUrl = config.LinkedInUrl?.Value,
            GitHubUrl = config.GitHubUrl?.Value,
            CvUrl = config.CvUrl?.Value
        };
    }
}