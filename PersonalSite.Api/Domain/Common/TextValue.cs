using PersonalSite.Api.Domain.Exceptions;


namespace PersonalSite.Api.Domain.Common;

public static class TextValue
{
    public static string Create(
        string? value,
        string fieldName,
        int minLength,
        int maxLength)
    {
        var cleaned = Normalize(value);

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException(
                $"{fieldName} is required.");
        }

        if (cleaned.Length < minLength)
        {
            throw new DomainException(
                $"{fieldName} must be at least {minLength} characters.");
        }

        if (cleaned.Length > maxLength)
        {
            throw new DomainException(
                $"{fieldName} cannot be longer than {maxLength} characters.");
        }

        if (cleaned.Any(char.IsControl))
        {
            throw new DomainException(
                $"{fieldName} contains invalid characters.");
        }

        if (!cleaned.Any(char.IsLetter))
        {
            throw new DomainException(
                $"{fieldName} must contain at least one letter.");
        }

        return cleaned;
    }

    private static string Normalize(string? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return string.Join(
            " ",
            value.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries));
    }
}