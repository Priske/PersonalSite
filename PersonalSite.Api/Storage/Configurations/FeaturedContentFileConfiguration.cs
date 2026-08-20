using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.FeaturedContent;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class FeaturedContentFileConfiguration
    : IEntityTypeConfiguration<FeaturedContentFile>
{
    public void Configure(
        EntityTypeBuilder<FeaturedContentFile> builder)
    {
        builder.HasKey(file => new
        {
            file.FeaturedContentId,
            file.StoredFileId
        });

        builder.HasOne(file => file.File)
            .WithMany()
            .HasForeignKey(file => file.StoredFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}