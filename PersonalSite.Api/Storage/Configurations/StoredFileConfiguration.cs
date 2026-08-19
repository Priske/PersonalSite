using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Files;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class StoredFileConfiguration
    : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(
         EntityTypeBuilder<StoredFile> builder)
    {
        builder.HasKey(file => file.Id);

        builder.Property(file => file.StorageKey)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(file => file.StorageKey)
            .IsUnique();

        builder.Property(file => file.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(file => file.ContentType)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(file => file.SizeInBytes)
            .IsRequired();
    }
}