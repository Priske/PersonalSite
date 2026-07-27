using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class HomePageConfigConfiguration
    : IEntityTypeConfiguration<HomePageConfig>
{
    public void Configure(EntityTypeBuilder<HomePageConfig> config)
    {
        config.HasKey(c => c.Id);

        config.Property(c => c.HeroEyebrow)
            .HasConversion(
                heroEyebrow => heroEyebrow.Value,
                value => new HeroEyebrow(value))
            .HasMaxLength(HeroEyebrow.MaxLength)
            .IsRequired();

        config.Property(c => c.HeroHeading)
            .HasConversion(
                heroHeading => heroHeading.Value,
                value => new HeroHeading(value))
            .HasMaxLength(HeroHeading.MaxLength)
            .IsRequired();

        config.Property(c => c.HeroSummary)
            .HasConversion(
                heroSummary => heroSummary.Value,
                value => new HeroSummary(value))
            .HasMaxLength(HeroSummary.MaxLength)
            .IsRequired();

        config.Property(c => c.ContactHeading)
            .HasConversion(
                contactHeading => contactHeading.Value,
                value => new ContactHeading(value))
            .HasMaxLength(ContactHeading.MaxLength)
            .IsRequired();
        config.Property(c => c.ContactDescription)
            .HasConversion(
                contactDescription => contactDescription.Value,
                value => new ContactDescription(value))
            .HasMaxLength(ContactDescription.MaxLength)
            .IsRequired();

        config.Property(c => c.Email)
            .HasConversion(
                email => email.Value,
                value => new EmailAddress(value))
            .IsRequired();

        config.Property(c => c.PhoneNumber)
            .HasConversion(
                phoneNumber => phoneNumber == null ? null : phoneNumber.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : new PhoneNumber(value));

        config.Property(c => c.LinkedInUrl)
               .HasConversion(
                   url => url == null ? null : url.Value,
                   value => string.IsNullOrWhiteSpace(value) ? null : new Url(value));

        config.Property(c => c.GitHubUrl)
            .HasConversion(
                url => url == null ? null : url.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : new Url(value));

        config.Property(c => c.CvUrl)
            .HasConversion(
                url => url == null ? null : url.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : new Url(value));

    }
}