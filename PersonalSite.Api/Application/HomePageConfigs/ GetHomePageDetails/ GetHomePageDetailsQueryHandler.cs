using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Application.Projects.GetHomePageDetails;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;

public class GetHomePageDetailsQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetHomePageConfigDetailsResponse?> Execute(CancellationToken cancellationToken)
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
            HeroEyebrow = config.HeroEyebrow,
            HeroHeading = config.HeroHeading,
            HeroSummary = config.HeroSummary,
            ContactHeading = config.ContactHeading,
            ContactDescription = config.ContactDescription,
            Email = config.Email,
            PhoneNumber = config.PhoneNumber?.Value,
            LinkedInUrl = config.LinkedInUrl?.Value,
            GitHubUrl = config.GitHubUrl?.Value,
            CvUrl = config.CvUrl?.Value
        };
    }
}