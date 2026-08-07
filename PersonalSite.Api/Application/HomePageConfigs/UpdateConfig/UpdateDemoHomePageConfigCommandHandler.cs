using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.HomePageConfigs;
using PersonalSite.Api.Storage.HomePageConfigs;

namespace PersonalSite.Api.Application.HomePageConfigs.UpdateConfig;

public sealed class UpdateDemoHomePageConfigCommandHandler(
    IHomePageConfigRepository configRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        UpdateHomePageConfigRequest request,
        CancellationToken cancellationToken)
    {
        var config =
            await configRepository.GetDemoAsync(
                actor.UserId,
                cancellationToken);

        if (config is null)
        {
            return false;
        }

        config.HeroBanner = new HomePageText(request.HeroBanner, "Hero Banner");
        config.HeroFirstName = new HomePageText(request.HeroFirstName, "Hero First Name");
        config.HeroLastName = new HomePageText(request.HeroLastName, "Hero Last Name");
        config.HeroRole = new HomePageText(request.HeroRole, "Hero Role");

        config.HeroEyebrow = new HeroEyebrow(request.HeroEyebrow);
        config.HeroHeading = new HeroHeading(request.HeroHeading);
        config.HeroSummary = new HeroSummary(request.HeroSummary);

        config.HeroPrimaryActionLabel =
            new HomePageText(request.HeroPrimaryActionLabel, "Hero Primary Action Label");

        config.HeroSecondaryActionLabel = new HomePageText(request.HeroSecondaryActionLabel, "Hero Secondary Action Label");

        config.ContactSectionNumber = new SectionNumber(request.ContactSectionNumber);

        config.ContactSectionEyebrow = new HomePageText(request.ContactSectionEyebrow, "Contact Section Eyebrow");
        config.ContactSectionHeading = new HomePageText(request.ContactSectionHeading, "Contact Section Heading");
        config.ContactEyebrow = new HomePageText(request.ContactEyebrow, "Contact Eyebrow");

        config.ContactHeading = new ContactHeading(request.ContactHeading);
        config.ContactDescription = new ContactDescription(request.ContactDescription);

        config.ContactEmailActionLabel = new HomePageText(request.ContactEmailActionLabel, "Contact Email Action Label");
        config.ContactLoginActionLabel = new HomePageText(request.ContactLoginActionLabel, "Contact Login Action Label");

        config.Email = new EmailAddress(request.Email);

        config.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : new PhoneNumber(request.PhoneNumber);

        config.LinkedInUrl = string.IsNullOrWhiteSpace(request.LinkedInUrl)
            ? null
            : new Url(request.LinkedInUrl);

        config.GitHubUrl = string.IsNullOrWhiteSpace(request.GitHubUrl)
            ? null
            : new Url(request.GitHubUrl);

        config.CvUrl = string.IsNullOrWhiteSpace(request.CvUrl)
            ? null
            : new Url(request.CvUrl);

        await configRepository.SaveChangesAsync(cancellationToken);

        config.LastEditedByUserId = actor.UserId;
        config.LastEditedAt = DateTimeOffset.UtcNow;

        await configRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}