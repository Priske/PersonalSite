using Microsoft.AspNetCore.Identity;
using PersonalSite.Api.Domain.Skills;
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

        if (string.IsNullOrWhiteSpace(settings.Password))
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

        var email = new UserEmail(settings.Email);

        var exists = dbContext.Users.Any(user =>
            (string)user.Email == email.Value);

        if (exists)
        {
            return;
        }

        var administrator = new User
        {
            Name = new UserName(settings.Name),
            Email = email,
            PasswordHash = string.Empty,
            Role = UserRole.Administrator
        };

        administrator.PasswordHash =
            passwordHasher.HashPassword(
                administrator,
                settings.Password);

        Console.WriteLine(
            $"Admin email: {settings.Email}");

        dbContext.Users.Add(administrator);
        dbContext.SaveChanges();
    }

    public static void SeedSkills(
        AppDbContext dbContext)
    {
        if (dbContext.SkillGroups.Any())
        {
            return;
        }

        var backend = new SkillGroup
        {
            Name = new SkillGroupName("Backend"),
            DisplayOrder = 1
        };

        var frontend = new SkillGroup
        {
            Name = new SkillGroupName("Frontend"),
            DisplayOrder = 2
        };

        var data = new SkillGroup
        {
            Name = new SkillGroupName("Data"),
            DisplayOrder = 3
        };

        var workflow = new SkillGroup
        {
            Name = new SkillGroupName("Workflow"),
            DisplayOrder = 4
        };

        dbContext.SkillGroups.AddRange(
            backend,
            frontend,
            data,
            workflow);

        dbContext.SaveChanges();

        dbContext.Skills.AddRange(
            new Skill
            {
                SkillGroupId = backend.Id,
                SkillName = new SkillName("C#"),
                DisplayOrder = 1
            },
            new Skill
            {
                SkillGroupId = backend.Id,
                SkillName = new SkillName("ASP.NET Core"),
                DisplayOrder = 2
            },
            new Skill
            {
                SkillGroupId = backend.Id,
                SkillName = new SkillName("Minimal APIs"),
                DisplayOrder = 3
            },
            new Skill
            {
                SkillGroupId = backend.Id,
                SkillName = new SkillName("Entity Framework Core"),
                DisplayOrder = 4
            },

            new Skill
            {
                SkillGroupId = frontend.Id,
                SkillName = new SkillName("React"),
                DisplayOrder = 1
            },
            new Skill
            {
                SkillGroupId = frontend.Id,
                SkillName = new SkillName("TypeScript"),
                DisplayOrder = 2
            },
            new Skill
            {
                SkillGroupId = frontend.Id,
                SkillName = new SkillName("HTML"),
                DisplayOrder = 3
            },
            new Skill
            {
                SkillGroupId = frontend.Id,
                SkillName = new SkillName("CSS"),
                DisplayOrder = 4
            },

            new Skill
            {
                SkillGroupId = data.Id,
                SkillName = new SkillName("SQL"),
                DisplayOrder = 1
            },
            new Skill
            {
                SkillGroupId = data.Id,
                SkillName = new SkillName("SQLite"),
                DisplayOrder = 2
            },
            new Skill
            {
                SkillGroupId = data.Id,
                SkillName =
                    new SkillName(
                        "Relational database design"),
                DisplayOrder = 3
            },

            new Skill
            {
                SkillGroupId = workflow.Id,
                SkillName = new SkillName("Git"),
                DisplayOrder = 1
            },
            new Skill
            {
                SkillGroupId = workflow.Id,
                SkillName = new SkillName("REST APIs"),
                DisplayOrder = 2
            },
            new Skill
            {
                SkillGroupId = workflow.Id,
                SkillName = new SkillName("Testing"),
                DisplayOrder = 3
            },
            new Skill
            {
                SkillGroupId = workflow.Id,
                SkillName =
                    new SkillName("Clean architecture"),
                DisplayOrder = 4
            });

        dbContext.SaveChanges();
    }
}