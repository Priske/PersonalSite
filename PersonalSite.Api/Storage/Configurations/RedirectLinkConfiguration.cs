using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Redirects;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class RedirectLinkConfiguration : IEntityTypeConfiguration<RedirectLink>
{
    public void Configure(EntityTypeBuilder<RedirectLink> builder)
    {
        builder.HasKey(link => link.Id);
        builder.Property(link => link.Slug)
            .HasConversion(slug => slug.Value, value => new RedirectSlug(value))
            .HasMaxLength(RedirectSlug.MaxLength).IsRequired();
        builder.HasIndex(link => link.Slug).IsUnique();
        builder.Property(link => link.TargetUrl)
            .HasConversion(target => target.Value, value => new RedirectTargetUrl(value))
            .HasMaxLength(RedirectTargetUrl.MaxLength).IsRequired();
        builder.Property(link => link.Campaign)
            .HasConversion(campaign => campaign == null ? null : campaign.Value,
                value => value == null ? null : new RedirectCampaign(value))
            .HasMaxLength(RedirectCampaign.MaxLength);
    }
}
