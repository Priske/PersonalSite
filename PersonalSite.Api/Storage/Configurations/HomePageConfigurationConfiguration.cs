using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class HomePageConfigConfiguration : IEntityTypeConfiguration<HomePageConfig>
{
    public void Configure(EntityTypeBuilder<HomePageConfig> config)
    {
        config.HasKey(homePageConfig => homePageConfig.Id);


        config.ConfigureSiteContent();

        config.Property(homePageConfig => homePageConfig.HeroBanner)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Hero Banner"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();


        config.Property(homePageConfig => homePageConfig.HeroBanner)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Hero Banner"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.HeroFirstName)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Hero First Name"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.HeroLastName)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Hero Last Name"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.HeroRole)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Hero Role"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.HeroEyebrow)
            .HasConversion(
                value => value.Value,
                value => new HeroEyebrow(value))
            .HasMaxLength(HeroEyebrow.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.HeroHeading)
            .HasConversion(
                value => value.Value,
                value => new HeroHeading(value))
            .HasMaxLength(HeroHeading.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.HeroSummary)
            .HasConversion(
                value => value.Value,
                value => new HeroSummary(value))
            .HasMaxLength(HeroSummary.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.HeroPrimaryActionLabel)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Hero Primary Action Label"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.HeroSecondaryActionLabel)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Hero Secondary Action Label"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(config => config.ContactSectionNumber)
            .HasConversion(
                value => value.Value,
                value => new SectionNumber(value))
            .HasMaxLength(SectionNumber.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.ContactSectionEyebrow)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Contact Section Eyebrow"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.ContactSectionHeading)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Contact Section Heading"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.ContactEyebrow)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Contact Eyebrow"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.ContactHeading)
            .HasConversion(
                value => value.Value,
                value => new ContactHeading(value))
            .HasMaxLength(ContactHeading.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.ContactDescription)
            .HasConversion(
                value => value.Value,
                value => new ContactDescription(value))
            .HasMaxLength(ContactDescription.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.ContactEmailActionLabel)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Contact Email Action Label"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.ContactLoginActionLabel)
            .HasConversion(
                value => value.Value,
                value => new HomePageText(value, "Contact Login Action Label"))
            .HasMaxLength(HomePageText.MaxLength)
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.Email)
            .HasConversion(
                value => value.Value,
                value => new EmailAddress(value))
            .IsRequired();

        config.Property(homePageConfig => homePageConfig.PhoneNumber)
            .HasConversion(
                value => value == null ? null : value.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : new PhoneNumber(value));

        config.Property(homePageConfig => homePageConfig.LinkedInUrl)
            .HasConversion(
                value => value == null ? null : value.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : new Url(value));

        config.Property(homePageConfig => homePageConfig.GitHubUrl)
            .HasConversion(
                value => value == null ? null : value.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : new Url(value));

        config.Property(homePageConfig => homePageConfig.CvUrl)
            .HasConversion(
                value => value == null ? null : value.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : new Url(value));
    }
}