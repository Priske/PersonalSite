using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

internal sealed class StringMetadataValueConfiguration
    : IEntityTypeConfiguration<StringMetadataValueEntity>
{
    public void Configure(
        EntityTypeBuilder<StringMetadataValueEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Value)
            .IsRequired();
    }
}