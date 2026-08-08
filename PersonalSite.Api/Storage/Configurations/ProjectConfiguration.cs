using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class ProjectConfiguration
    : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> project)
    {
        project.HasKey(p => p.Id);

        project.ConfigureSiteContent();

        project.Property(p => p.Title)
            .HasConversion(
                title => title.Value,
                value => new ProjectTitle(value))
            .HasMaxLength(ProjectTitle.MaxLength)
            .IsRequired();

        project.Property(p => p.Description)
            .HasConversion(
                description => description.Value,
                value => new ProjectDescription(value))
            .HasMaxLength(ProjectDescription.MaxLength)
            .IsRequired();

        project.Property(p => p.RepositoryUrl)
            .HasConversion(
                url => url.Value,
                value => new Url(value))
            .IsRequired();

        project.Property(p => p.LiveUrl)
            .HasConversion(
                url => url == null ? null : url.Value,
                value => string.IsNullOrWhiteSpace(value)
                    ? null
                    : new Url(value));

        project.Property(p => p.IsFeatured)
            .IsRequired();

        project.Property(p => p.DisplayOrder)
            .IsRequired();

        project.HasIndex(p => p.DisplayOrder)
            .HasFilter("\"Source\" = 0")
            .IsUnique();

        project.HasIndex(p => new
        {
            p.Created.UserId,
            p.DisplayOrder
        })
            .HasFilter("\"Source\" = 1")
            .IsUnique();

        project.HasMany(p => p.Tags)
            .WithMany(t => t.Projects);
    }
}
