using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Security;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Seeding;

public static class DatabaseSeeder
{
    public static void SeedUsers(
        AppDbContext dbContext,
        UserFuzzr userFuzzr,
        int count = 50)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (dbContext.Users.Any())
        {
            return;
        }

        var users = userFuzzr
            .ManyAsync(count)
            .GetAwaiter()
            .GetResult();

        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();
    }

    public static void SeedAdministrator(
    AppDbContext dbContext,
    IConfiguration configuration,
    IPasswordHasher<User> passwordHasher)
    {
        var settings = configuration
           .GetRequiredSection(DevelopmentAdminSettings.SectionName)
           .Get<DevelopmentAdminSettings>()
           ?? throw new InvalidOperationException(
               "DevelopmentAdmin settings are missing.");

        if (settings is null ||
            string.IsNullOrWhiteSpace(settings.Password))
        {
            return;
        }


        if (string.IsNullOrWhiteSpace(settings.Name))
        {
            throw new InvalidOperationException(
                "DevelopmentAdmin:Name is missing.");
        }

        if (string.IsNullOrWhiteSpace(settings.Email))
        {
            throw new InvalidOperationException(
                "DevelopmentAdmin:Email is missing.");
        }

        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            throw new InvalidOperationException(
                "DevelopmentAdmin:Password is missing.");
        }

        var email =
            new UserEmail(settings.Email);

        var exists =
            dbContext.Users.Any(user =>
                (string)user.Email == email.Value);

        if (exists)
        {
            return;
        }

        var administrator =
            new User
            {
                Name =
                    new UserName(settings.Name),
                Email = email,
                PasswordHash = string.Empty,
                Role = UserRole.Administrator
            };

        administrator.PasswordHash =
            passwordHasher.HashPassword(
                administrator,
                settings.Password);
        Console.WriteLine("Admin E-mail: " + settings.Email);
        Console.WriteLine("Admin pasword: " + settings.Password);
        dbContext.Users.Add(administrator);
        dbContext.SaveChanges();
    }
}