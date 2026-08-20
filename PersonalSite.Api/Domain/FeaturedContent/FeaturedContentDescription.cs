using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.FeaturedContent;

public sealed record FeaturedContentDescription
{
    public const int MinLength = 5;
    public const int MaxLength = 500;

    public string Value { get; }

    public FeaturedContentDescription(string value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "FeaturedContentDescription",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(FeaturedContentDescription featuredContentDescription)
        => featuredContentDescription.Value;

    public override string ToString()
        => Value;
}