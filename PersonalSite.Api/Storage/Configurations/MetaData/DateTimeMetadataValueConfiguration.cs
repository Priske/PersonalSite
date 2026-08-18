using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

internal sealed class DateTimeMetadataValueConfiguration
    : IEntityTypeConfiguration<DateTimeMetadataValueEntity>
{
    public void Configure(
        EntityTypeBuilder<DateTimeMetadataValueEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .IsRequired();
    }
}