using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> user)
    {
        user.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new UserEmail(value))
            .HasMaxLength(EmailValue.MaxLength)
            .IsRequired();

        user.HasIndex(u => u.Email)
            .IsUnique();

        user.Property(u => u.Name)
            .HasConversion(
                name => name.Value,
                value => new UserName(value))
            .HasMaxLength(UserName.MaxLength)
            .IsRequired();

        user.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

    }
}