using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

internal sealed class ObjectMetadataValueEntryConfiguration
    : IEntityTypeConfiguration<ObjectMetadataValueEntry>
{
    public void Configure(
        EntityTypeBuilder<ObjectMetadataValueEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .IsRequired();

        builder.Property(x => x.ValueType)
            .IsRequired();

        builder
            .HasOne<ObjectMetadataValueEntity>()
            .WithMany()
            .HasForeignKey(x => x.ObjectMetadataValueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}