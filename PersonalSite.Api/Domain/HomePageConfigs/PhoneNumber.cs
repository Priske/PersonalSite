using PhoneNumbers;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public sealed record PhoneNumber
{
    private static readonly PhoneNumberUtil PhoneUtil =
        PhoneNumberUtil.GetInstance();

    public string Value { get; }

    public PhoneNumber(string value)
    {
        var cleaned = value.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            throw new DomainException("Phone number is required.");
        }

        try
        {
            // "BE" is the default region when no country code is supplied.
            var parsed = PhoneUtil.Parse(cleaned, "BE");

            if (!PhoneUtil.IsValidNumber(parsed))
            {
                throw new DomainException("Phone number is invalid.");
            }

            Value = PhoneUtil.Format(
                parsed,
                PhoneNumberFormat.E164);
        }
        catch (NumberParseException)
        {
            throw new DomainException("Phone number is invalid.");
        }
    }

    public static implicit operator string(PhoneNumber phoneNumber)
        => phoneNumber.Value;

    public override string ToString()
        => Value;
}