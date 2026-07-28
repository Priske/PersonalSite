using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record HeroSummary
{
    public const int MinLength = 2;
    public const int MaxLength = 500;

    public string Value { get; }

    public HeroSummary(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Hero Summary",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(HeroSummary heroSummary)
        => heroSummary.Value;

    public override string ToString()
        => Value;
}