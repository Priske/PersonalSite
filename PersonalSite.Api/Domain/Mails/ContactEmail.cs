using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Mails;

public sealed record ContactEmail
{
    public string Value { get; }

    public ContactEmail(string? value)
    {
        Value = EmailValue.Create(value, "Contact email");
    }

    public static implicit operator string(ContactEmail email)
    => email.Value;

    public override string ToString()
        => Value;
}