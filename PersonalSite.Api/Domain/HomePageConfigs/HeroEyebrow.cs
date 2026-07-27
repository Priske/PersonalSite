using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record HeroEyebrow
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public HeroEyebrow(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Hero Eyebrow",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(HeroEyebrow heroEyebrow)
        => heroEyebrow.Value;

    public override string ToString()
        => Value;
}