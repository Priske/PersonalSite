using Microsoft.AspNetCore.Identity;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.HomePageConfigs;
using PersonalSite.Api.Domain.Skills;
using PersonalSite.Api.Domain.Tags;
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

    public static User? SeedInitialAdministrator(
       AppDbContext dbContext,
       IConfiguration configuration,
       IPasswordHasher<User> passwordHasher)
    {
        var settings = configuration
            .GetRequiredSection(InitialAdminSettings.SectionName)
            .Get<InitialAdminSettings>()
            ?? throw new InvalidOperationException(
                "InitialAdmin settings are missing.");

        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(settings.Name))
        {
            throw new InvalidOperationException(
                "InitialAdmin:Name is missing.");
        }

        if (string.IsNullOrWhiteSpace(settings.Email))
        {
            throw new InvalidOperationException(
                "InitialAdmin:Email is missing.");
        }

        var email = new UserEmail(settings.Email);

        var existingAdministrator = dbContext.Users
            .SingleOrDefault(user =>
                (string)user.Email == email.Value);

        if (existingAdministrator is not null)
        {
            return existingAdministrator;
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

        dbContext.Users.Add(administrator);
        dbContext.SaveChanges();

        return administrator;
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


    public static IReadOnlyList<Tag> SeedTags(
        AppDbContext dbContext,
        TagFuzzr tagFuzzr,
        int adminUserId,
        int count = 10)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (dbContext.Tags.Any())
        {
            return dbContext.Tags.ToList();
        }

        var tags = tagFuzzr.Many(
            count,
            adminUserId);

        dbContext.Tags.AddRange(tags);
        dbContext.SaveChanges();

        return tags;
    }

    public static void SeedProjects(
        AppDbContext dbContext,
        ProjectFuzzr projectFuzzr,
        IReadOnlyList<Tag> tags,
        int count = 10)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (dbContext.Projects.Any())
        {
            return;
        }

        var projects = projectFuzzr.Many(
            count,
            tags);

        dbContext.Projects.AddRange(projects);
        dbContext.SaveChanges();
    }

    public static void SeedHomePageConfig(
    AppDbContext dbContext,
    int administratorId)
    {
        if (dbContext.HomepageConfigs.Any())
        {
            return;
        }

        var config = new HomePageConfig(
            ContentSource.Official,
            administratorId)
        {
            HeroBanner = new HomePageText(
                "Software developer",
                "Hero Banner"),

            HeroFirstName = new HomePageText(
                "Ben",
                "Hero First Name"),

            HeroLastName = new HomePageText(
                "Eeckman",
                "Hero Last Name"),

            HeroRole = new HomePageText(
                "Junior software developer",
                "Hero Role"),

            HeroEyebrow = new HeroEyebrow(
                "Practical software. Clear structure."),

            HeroHeading = new HeroHeading(
                "I build maintainable applications for the web."),

            HeroSummary = new HeroSummary(
                "I work with C#, ASP.NET Core, React, TypeScript and SQL to create software that is understandable, useful and easy to develop further."),

            HeroPrimaryActionLabel = new HomePageText(
                "View projects",
                "Hero Primary Action Label"),

            HeroSecondaryActionLabel = new HomePageText(
                "Contact me",
                "Hero Secondary Action Label"),

            ContactSectionNumber = new SectionNumber(
                "03"),

            ContactSectionEyebrow = new HomePageText(
                "Get in touch",
                "Contact Section Eyebrow"),

            ContactSectionHeading = new HomePageText(
                "Contact",
                "Contact Section Heading"),

            ContactEyebrow = new HomePageText(
                "Have a project or opportunity?",
                "Contact Eyebrow"),

            ContactHeading = new ContactHeading(
                "Let's talk."),

            ContactDescription = new ContactDescription(
                "I am interested in junior software-development roles, practical projects and opportunities to continue developing my skills."),

            ContactEmailActionLabel = new HomePageText(
                "Send an email",
                "Contact Email Action Label"),

            ContactLoginActionLabel = new HomePageText(
                "Account login",
                "Contact Login Action Label"),

            Email = new EmailAddress(
                "eeckman_ben@hotmail.com"),

            PhoneNumber = new PhoneNumber(
                "+32 485 86 19 15"),

            LinkedInUrl = new Url(
                "https://www.linkedin.com/in/ben-eeckman-11b5a1418/"),

            GitHubUrl = new Url(
                "https://github.com/Priske"),

            CvUrl = new Url(
                "https://media.licdn.com/dms/image/v2/D4D2DAQE-hWDW6A2dxQ/profile-treasury-document-images_1280/B4DZ.ob7WGLAAg-/1/1785237322476?e=1785974400&v=beta&t=ef5BDgiLD4KSDDXdnb7RKmXgKI_O2tONjRe4yoHKq6c")
        };

        dbContext.HomepageConfigs.Add(config);
        dbContext.SaveChanges();
    }
}