using PersonalSite.Api.Domain.Tags;
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

    public IReadOnlyList<Tag> Many(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        return CreateFuzzr()
            .Many(count)
            .Generate()
            .Select((name, index) => new Tag
            {
                Name = new TagName($"{name}-{index}")
            })
            .ToList();
    }

    private FuzzrOf<string> CreateFuzzr()
        => Fuzzr.OneOf(Names);
}