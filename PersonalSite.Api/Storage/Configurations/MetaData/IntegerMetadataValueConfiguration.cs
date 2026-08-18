using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

internal sealed class IntegerMetadataValueConfiguration
    : IEntityTypeConfiguration<IntegerMetadataValueEntity>
{
    public void Configure(
        EntityTypeBuilder<IntegerMetadataValueEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .IsRequired();
    }
}
