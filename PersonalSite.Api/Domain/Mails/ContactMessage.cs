using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Mails;

public sealed record ContactMessage
{
    public const int MinLength = 10;
    public const int MaxLength = 5000;

    public string Value { get; }

    public ContactMessage(string? value)
    {
        var cleaned = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException(
                "Contact message is required.");
        }

        if (cleaned.Length < MinLength)
        {
            throw new DomainException(
                $"Contact message must be at least {MinLength} characters.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException(
                $"Contact message cannot be longer than {MaxLength} characters.");
        }

        if (cleaned.Any(character =>
                char.IsControl(character) &&
                character is not '\r' and not '\n' and not '\t'))
        {
            throw new DomainException(
                "Contact message contains invalid characters.");
        }

        Value = cleaned;
    }

    public static implicit operator string(ContactMessage ContactMessage)
        => ContactMessage.Value;

    public override string ToString()
        => Value;
}