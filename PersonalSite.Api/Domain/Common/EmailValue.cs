using System.Net.Mail;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Common;

public static class EmailValue
{
    public const int MaxLength = 254;

    public static string Create(
        string? value,
        string fieldName = "Email")
    {
        var cleaned = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException(
                $"{fieldName} is required.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException(
                $"{fieldName} cannot be longer than " +
                $"{MaxLength} characters.");
        }

        if (!MailAddress.TryCreate(cleaned, out var parsed) ||
            !string.Equals(
                parsed.Address,
                cleaned,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException(
                $"{fieldName} is not valid.");
        }

        return cleaned.ToLowerInvariant();
    }
}