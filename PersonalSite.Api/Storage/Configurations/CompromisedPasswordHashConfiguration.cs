using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Infrastructure.Security.Password;

namespace PersonalSite.Api.Storage.Configurations;


public sealed class CompromisedPasswordHashConfiguration
    : IEntityTypeConfiguration<CompromisedPasswordHash>
{
    public void Configure(
        EntityTypeBuilder<CompromisedPasswordHash> builder)
    {
        builder.ToTable("CompromisedPasswordHashes");

        builder.HasKey(x => x.Hash);

        builder.Property(x => x.Hash)
            .HasMaxLength(40)
            .IsFixedLength()
            .IsRequired();

        builder.Property(x => x.OccurrenceCount)
            .IsRequired();
    }
}