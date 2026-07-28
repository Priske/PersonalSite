using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record HomePageText
{
    public const int MinLength = 1;
    public const int MaxLength = 200;

    public string Value { get; }

    public HomePageText(string? value, string fieldName)
    {
        Value = TextValue.Create(value, fieldName, MinLength, MaxLength);
    }

    public static implicit operator string(HomePageText text) => text.Value;

    public override string ToString() => Value;
}