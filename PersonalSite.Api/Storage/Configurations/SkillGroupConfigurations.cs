using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class SkillGroupConfiguration
    : IEntityTypeConfiguration<SkillGroup>
{
    public void Configure(EntityTypeBuilder<SkillGroup> skillGroup)
    {
        skillGroup.HasKey(group => group.Id);

        skillGroup.Property(group => group.Name)
            .HasConversion(
                name => name.Value,
                value => new SkillGroupName(value))
            .HasMaxLength(SkillGroupName.MaxLength)
            .IsRequired();

        skillGroup.HasIndex(group => group.DisplayOrder)
            .IsUnique();
    }
}