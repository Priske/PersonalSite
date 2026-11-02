using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Redirects;

public sealed record RedirectCampaign
{
    public const int MaxLength = 200;
    public const string ValidationMessage = "Use a single-line campaign of up to 200 characters.";

    public string Value { get; }

    public RedirectCampaign(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException(ValidationMessage);
        ValidateFormat(value);
        Value = value.Trim();
    }

    public static RedirectCampaign? CreateOptional(string? value)
    {
        if (value is null) return null;
        // Validate before treating whitespace as absent: controls/overlength must
        // still be rejected, exactly as for a nonblank request.
        ValidateFormat(value);
        return string.IsNullOrWhiteSpace(value) ? null : new RedirectCampaign(value);
    }

    private static void ValidateFormat(string value)
    {
        if (value.Length > MaxLength || value.Any(char.IsControl))
            throw new DomainException(ValidationMessage);
    }

    public static implicit operator string(RedirectCampaign campaign) => campaign.Value;
    public override string ToString() => Value;
}
