using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using QuickFuzzr;

namespace PersonalSite.Api.Seeding;

public sealed class TagFuzzr
{
    private static readonly string[] Names =
    [
        "C#",
        ".NET",
        "ASP.NET Core",
        "Entity Framework Core",
        "React",
        "TypeScript",
        "JavaScript",
        "SQL",
        "SQLite",
        "Git",
        "REST APIs",
        "Testing"
    ];

    public IReadOnlyList<Tag> Many(
        int count,
        int userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var actor = new Actor(
            userId,
            UserRole.Administrator);

        return CreateFuzzr()
            .Many(count)
            .Generate()
            .Select((name, index) =>
                Tag.Create(
                    actor,
                    new TagName($"{name}-{index}")))
            .ToList();
    }

    private FuzzrOf<string> CreateFuzzr()
        => Fuzzr.OneOf(Names);
}