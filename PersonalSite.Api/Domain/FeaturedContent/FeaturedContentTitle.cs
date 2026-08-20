using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.FeaturedContent;

public sealed record FeaturedContentTitle
{
    public const int MinLength = 5;
    public const int MaxLength = 250;

    public string Value { get; }

    public FeaturedContentTitle(string value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "FeaturedContentTitle",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(FeaturedContentTitle featuredContentTitle)
        => featuredContentTitle.Value;

    public override string ToString()
        => Value;
}
