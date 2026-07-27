using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record ContactDescription
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public ContactDescription(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Contact Description",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(ContactDescription contactDescription)
        => contactDescription.Value;

    public override string ToString()
        => Value;
}