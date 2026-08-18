using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

internal sealed class DecimalMetadataValueConfiguration
    : IEntityTypeConfiguration<DecimalMetadataValueEntity>
{
    public void Configure(
        EntityTypeBuilder<DecimalMetadataValueEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .HasPrecision(18, 4)
            .IsRequired();
    }
}
