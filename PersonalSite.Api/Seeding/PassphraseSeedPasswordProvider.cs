using QuickFuzzr;

namespace PersonalSite.Api.Seeding;

public sealed class PassphraseSeedPasswordProvider
    : ISeedPasswordProvider
{
    private static readonly string[] Words =
    [
        "badger",
        "umbrella",
        "database",
        "octopus",
        "spaceship",
        "cupcake",
        "philosopher",
        "typewriter"
    ];

    public FuzzrOf<string> Password =>
        from first in Fuzzr.OneOf(Words)
        from second in Fuzzr.OneOf(Words)
        from third in Fuzzr.OneOf(Words)
        from fourth in Fuzzr.OneOf(Words)
        select $"{first} {second} {third} {fourth}";
}