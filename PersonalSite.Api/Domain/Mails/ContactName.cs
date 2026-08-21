using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Mails;

public sealed record ContactName
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public ContactName(string value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Contact name",
            minLength: MinLength,
            maxLength: MaxLength);
    }
    public static implicit operator string(ContactName name)
        => name.Value;

    public override string ToString()
        => Value;
}