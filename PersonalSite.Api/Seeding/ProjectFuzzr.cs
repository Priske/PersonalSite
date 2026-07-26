using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Tags;
using QuickFuzzr;

namespace PersonalSite.Api.Seeding;

public sealed class ProjectFuzzr
{
    private static readonly string[] Titles =
    [
        "Personal Portfolio",
        "Book Tracker",
        "Task Manager",
        "Budget Planner",
        "Inventory Manager",
        "Recipe Application",
        "Movie Library",
        "Contact Manager"
    ];

    private static readonly string[] Descriptions =
    [
        "A personal software project.",
        "A full-stack web application.",
        "A project focused on REST APIs.",
        "A CRUD application built for learning.",
        "A project using modern web technologies.",
        "A project focused on clean backend code."
    ];

    public IReadOnlyList<Project> Many(
        int count,
        IReadOnlyList<Tag> tags)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var seeds = CreateFuzzr()
            .Many(count)
            .Generate()
            .ToList();

        return seeds
            .Select((seed, index) => new Project
            {
                Title = new ProjectTitle(
                    $"{seed.Title} {index + 1}"),

                Description = new ProjectDescription(
                    seed.Description),

                RepositoryUrl = new Url(
                    $"https://github.com/example/project-{index + 1}"),

                LiveUrl = new Url(
                    $"https://example.com/project-{index + 1}"),

                IsFeatured = index < 3,
                DisplayOrder = index + 1,

                Tags = tags.Count == 0
                    ? []
                    : tags
                        .Skip(index % tags.Count)
                        .Take(Math.Min(3, tags.Count))
                        .ToList()
            })
            .ToList();
    }

    private FuzzrOf<ProjectSeedData> CreateFuzzr() =>
        from title in Fuzzr.OneOf(Titles)
        from description in Fuzzr.OneOf(Descriptions)
        select new ProjectSeedData(
            title,
            description);

    private sealed record ProjectSeedData(
        string Title,
        string Description);
}