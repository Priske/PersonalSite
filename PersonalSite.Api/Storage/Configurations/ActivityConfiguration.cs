using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Analytics;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class ActivityConfiguration
    : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasConversion<string>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasMany(x => x.Metadata)
            .WithOne()
            .HasForeignKey("ActivityId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Metadata)
            .HasField("_metadata")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}