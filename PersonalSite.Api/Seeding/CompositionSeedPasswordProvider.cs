using QuickFuzzr;

namespace PersonalSite.Api.Seeding;

public sealed class CompositionSeedPasswordProvider
    : ISeedPasswordProvider
{
    private static readonly string[] Words =
    [
        "Badger",
        "Umbrella",
        "Database",
        "Octopus",
        "Spaceship",
        "Cupcake"
    ];

    private static readonly string[] Symbols =
    [
        "!",
        "@",
        "#",
        "$"
    ];

    private static readonly string[] Numbers =
    [
        "17",
        "28",
        "42",
        "73"
    ];

    public FuzzrOf<string> Password =>
        from first in Fuzzr.OneOf(Words)
        from second in Fuzzr.OneOf(Words)
        from number in Fuzzr.OneOf(Numbers)
        from symbol in Fuzzr.OneOf(Symbols)
        select $"{first}{second}{number}{symbol}";
}