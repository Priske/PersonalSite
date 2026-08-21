using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record EmailAddress
{
    public const int MaxLength = EmailValue.MaxLength;

    public string Value { get; }

    public EmailAddress(string? value)
    {
        Value = EmailValue.Create(
            value,
            "Email");
    }

    public static implicit operator string(EmailAddress email)
        => email.Value;

    public override string ToString()
        => Value;
}