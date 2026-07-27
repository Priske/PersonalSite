using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.HomePageConfigs;
using PersonalSite.Api.Storage.HomePageConfigs;

namespace PersonalSite.Api.Application.HomePageConfigs.UpdateConfig;

public class UpdateHomePageConfigCommandHandler(
    IHomePageConfigRepository configRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        UpdateHomePageConfigRequest request,
        CancellationToken cancellationToken)
    {
        HomePageConfigPermissions.EnsureCanManage(actor);

        var config = await configRepository.GetAsync(cancellationToken);

        if (config is null)
        {
            return false;
        }

        config.HeroEyebrow =
            new HeroEyebrow(request.HeroEyebrow);

        config.HeroHeading =
            new HeroHeading(request.HeroHeading);

        config.HeroSummary =
            new HeroSummary(request.HeroSummary);

        config.ContactHeading =
            new ContactHeading(request.ContactHeading);

        config.ContactDescription =
            new ContactDescription(request.ContactDescription);

        config.Email =
            new EmailAddress(request.Email);

        config.PhoneNumber =
            string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : new PhoneNumber(request.PhoneNumber);

        config.LinkedInUrl =
            string.IsNullOrWhiteSpace(request.LinkedInUrl)
                ? null
                : new Url(request.LinkedInUrl);

        config.GitHubUrl =
            string.IsNullOrWhiteSpace(request.GitHubUrl)
                ? null
                : new Url(request.GitHubUrl);

        config.CvUrl =
            string.IsNullOrWhiteSpace(request.CvUrl)
                ? null
                : new Url(request.CvUrl);

        await configRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}