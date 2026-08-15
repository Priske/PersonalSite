using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Analytics.Metadata;

namespace PersonalSite.Api.Storage.Configurations.MetaData;

public sealed class ActivityMetadataConfiguration
    : IEntityTypeConfiguration<ActivityMetadata>
{
    public void Configure(
        EntityTypeBuilder<ActivityMetadata> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.Values);
    }
}