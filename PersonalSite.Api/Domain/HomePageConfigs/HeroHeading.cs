using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record HeroHeading
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public HeroHeading(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Hero Heading",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(HeroHeading heroHeading)
        => heroHeading.Value;

    public override string ToString()
        => Value;
}