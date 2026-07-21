using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class SkillConfiguration
    : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> skill)
    {
        skill.HasKey(s => s.Id);

        skill.Property(s => s.SkillName)
            .HasConversion(
                skillname => skillname.Value,
                value => new SkillName(value))
            .HasMaxLength(SkillName.MaxLength)
            .IsRequired();

        skill.HasOne(s => s.SkillGroup)
            .WithMany(group => group.Skills)
            .HasForeignKey(s => s.SkillGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        skill.HasIndex(s => new
        {
            s.SkillGroupId,
            s.DisplayOrder
        })
        .IsUnique();

    }
}