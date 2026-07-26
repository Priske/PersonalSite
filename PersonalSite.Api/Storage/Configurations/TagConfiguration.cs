using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class TagConfiguration
    : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> tag)
    {
        tag.HasKey(t => t.Id);

        tag.Property(t => t.Name)
            .HasConversion(
                name => name.Value,
                value => new TagName(value))
            .HasMaxLength(TagName.MaxLength)
            .IsRequired();

        tag.HasIndex(t => t.Name)
            .IsUnique();

        tag.HasMany(current => current.Projects)
            .WithMany(current => current.Tags);
    }
}