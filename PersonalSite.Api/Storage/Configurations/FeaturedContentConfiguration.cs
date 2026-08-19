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

        builder.Property(content => content.Title)
            .IsRequired();

        builder.Property(content => content.Description)
            .IsRequired();

        builder.HasMany(content => content.Files)
            .WithOne()
            .HasForeignKey(file => file.FeaturedContentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(content => content.Files)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(content => content.Tags)
            .WithMany();

        builder.Navigation(content => content.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}