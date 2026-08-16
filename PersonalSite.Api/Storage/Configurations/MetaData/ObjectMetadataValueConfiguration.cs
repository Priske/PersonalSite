using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

internal sealed class ObjectMetadataValueConfiguration
    : IEntityTypeConfiguration<ObjectMetadataValueEntity>
{
    public void Configure(
        EntityTypeBuilder<ObjectMetadataValueEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}