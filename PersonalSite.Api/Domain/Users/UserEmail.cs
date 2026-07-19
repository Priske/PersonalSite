using System.Net.Mail;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Users;

public sealed record UserEmail
{
    public const int MaxLength = 254;

    public string Value { get; }

    public UserEmail(string value)
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

    public static implicit operator string(UserEmail email)
        => email.Value;

    public override string ToString()
        => Value;
}