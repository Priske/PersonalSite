using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

internal sealed class BooleanMetadataValueConfiguration
    : IEntityTypeConfiguration<BooleanMetadataValueEntity>
{
    public void Configure(
        EntityTypeBuilder<BooleanMetadataValueEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .IsRequired();
    }
}
