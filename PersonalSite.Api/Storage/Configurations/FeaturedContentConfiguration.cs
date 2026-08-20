using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.FeaturedContent;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class FeaturedContentConfiguration
    : IEntityTypeConfiguration<FeaturedContent>
{
    public void Configure(
        EntityTypeBuilder<FeaturedContent> builder)
    {
        builder.HasKey(content => content.Id);

        builder.ConfigureSiteContent();

        builder.Property(content => content.Title)
            .HasConversion(
                title => title.Value,
                value => new FeaturedContentTitle(value))
            .HasMaxLength(FeaturedContentTitle.MaxLength)
            .IsRequired();

        builder.Property(content => content.Description)
            .HasConversion(
                description => description.Value,
                value => new FeaturedContentDescription(value))
            .HasMaxLength(FeaturedContentDescription.MaxLength)
            .IsRequired();

        builder.HasMany(content => content.Files)
            .WithOne()
            .HasForeignKey(file => file.FeaturedContentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(content => content.Tags)
            .WithMany();
    }
}