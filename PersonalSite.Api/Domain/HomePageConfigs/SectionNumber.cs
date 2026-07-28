using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record SectionNumber
{
    public const int MinLength = 1;
    public const int MaxLength = 10;

    public string Value { get; }

    public SectionNumber(string? value)
    {
        var cleaned = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Contact Section Number is required.");
        }

        if (cleaned.Length < MinLength)
        {
            throw new DomainException($"Contact Section Number must contain at least {MinLength} character.");
        }

        if (cleaned.Length > MaxLength)
        {
            throw new DomainException($"Contact Section Number cannot be longer than {MaxLength} characters.");
        }

        if (!cleaned.All(char.IsDigit))
        {
            throw new DomainException("Contact Section Number may only contain numbers.");
        }

        Value = cleaned;
    }

    public static implicit operator string(SectionNumber sectionNumber)
    {
        return sectionNumber.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}