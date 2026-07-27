using System.Net.Mail;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record EmailAddress
{
    public const int MaxLength = 254;

    public string Value { get; }

    public EmailAddress(string value)
    {
        var cleaned = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Email is required.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException(
                $"Email cannot be longer than {MaxLength} characters.");
        }

        if (!MailAddress.TryCreate(cleaned, out var parsed) ||
            !string.Equals(
                parsed.Address,
                cleaned,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException("Email is not valid.");
        }

        Value = cleaned.ToLowerInvariant();
    }

    public static implicit operator string(EmailAddress email)
        => email.Value;

    public override string ToString()
        => Value;
}