using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Mails;

public sealed record ContactMessage
{
    public const int MinLength = 10;
    public const int MaxLength = 5000;

    public string Value { get; }

    public ContactMessage(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Contact Message",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(ContactMessage ContactMessage)
        => ContactMessage.Value;

    public override string ToString()
        => Value;
}