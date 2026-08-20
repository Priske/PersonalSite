using Microsoft.AspNetCore.Identity;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Actors;
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
        AppDbContext dbContext,
        int administratorUserId)
    {
        if (dbContext.SkillGroups.Any())
        {
            return;
        }

        var actor = new Actor(
            administratorUserId,
            UserRole.Administrator);

        var backend = SkillGroup.Create(
            actor,
            new SkillGroupName("Backend"),
            1);

        var frontend = SkillGroup.Create(
            actor,
            new SkillGroupName("Frontend"),
            2);

        var data = SkillGroup.Create(
            actor,
            new SkillGroupName("Data"),
            3);

        var workflow = SkillGroup.Create(
            actor,
            new SkillGroupName("Workflow"),
            4);

        dbContext.SkillGroups.AddRange(
            backend,
            frontend,
            data,
            workflow);

        dbContext.SaveChanges();

        dbContext.Skills.AddRange(
            Skill.Create(
                actor,
                backend.Id,
                new SkillName("C#"),
                1),
            Skill.Create(
                actor,
                backend.Id,
                new SkillName("ASP.NET Core"),
                2),
            Skill.Create(
                actor,
                backend.Id,
                new SkillName("Minimal APIs"),
                3),
            Skill.Create(
                actor,
                backend.Id,
                new SkillName("Entity Framework Core"),
                4),

            Skill.Create(
                actor,
                frontend.Id,
                new SkillName("React"),
                1),
            Skill.Create(
                actor,
                frontend.Id,
                new SkillName("TypeScript"),
                2),
            Skill.Create(
                actor,
                frontend.Id,
                new SkillName("HTML"),
                3),
            Skill.Create(
                actor,
                frontend.Id,
                new SkillName("CSS"),
                4),

            Skill.Create(
                actor,
                data.Id,
                new SkillName("SQL"),
                1),
            Skill.Create(
                actor,
                data.Id,
                new SkillName("SQLite"),
                2),
            Skill.Create(
                actor,
                data.Id,
                new SkillName(
                    "Relational database design"),
                3),

            Skill.Create(
                actor,
                workflow.Id,
                new SkillName("Git"),
                1),
            Skill.Create(
                actor,
                workflow.Id,
                new SkillName("REST APIs"),
                2),
            Skill.Create(
                actor,
                workflow.Id,
                new SkillName("Testing"),
                3),
            Skill.Create(
                actor,
                workflow.Id,
                new SkillName("Clean architecture"),
                4));

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
