using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record ContactHeading
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public ContactHeading(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Contact Heading",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(ContactHeading contactHeading)
        => contactHeading.Value;

    public override string ToString()
        => Value;
}