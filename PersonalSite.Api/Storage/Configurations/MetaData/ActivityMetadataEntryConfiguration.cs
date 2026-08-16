using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

internal sealed class ActivityMetadataEntryConfiguration
    : IEntityTypeConfiguration<ActivityMetadataEntry>
{
    public void Configure(
        EntityTypeBuilder<ActivityMetadataEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .IsRequired();

        builder.Property(x => x.ValueType)
            .IsRequired();

        builder
            .HasOne<ActivityMetadata>()
            .WithMany()
            .HasForeignKey(x => x.ActivityMetadataId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}