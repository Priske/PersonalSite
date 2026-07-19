using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Users;

public sealed record UserName
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public UserName(string value)
    {
        var cleaned = Normalize(value);

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Name is required.");
        }

        if (cleaned.Length < MinLength)
        {
            throw new DomainException(
                $"Name must be at least {MinLength} characters.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException(
                $"Name cannot be longer than {MaxLength} characters.");
        }

        if (cleaned.Any(char.IsControl))
        {
            throw new DomainException(
                "Name contains invalid characters.");
        }

        if (!cleaned.Any(char.IsLetter))
        {
            throw new DomainException(
                "Name must contain at least one letter.");
        }

        Value = cleaned;
    }

    private static string Normalize(string? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return string.Join(
            " ",
            value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    public static implicit operator string(UserName userName)
        => userName.Value;

    public override string ToString()
        => Value;
}