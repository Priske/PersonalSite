using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Redirects;

public sealed record RedirectTargetUrl
{
    public const int MaxLength = 2048;
    public const string ValidationMessage = "Use an absolute HTTP(S) URL without credentials, whitespace or control characters (up to 2048 characters).";

    public string Value { get; }

    public RedirectTargetUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > MaxLength ||
            value.Any(c => char.IsControl(c) || char.IsWhiteSpace(c) || c == '\\') ||
            !Uri.TryCreate(value, UriKind.Absolute, out var target) ||
            target.Scheme is not ("http" or "https") || string.IsNullOrEmpty(target.Host) ||
            !string.IsNullOrEmpty(target.UserInfo))
            throw new DomainException(ValidationMessage);

        // Preserve the saved URL verbatim, including its query/fragment and encoding.
        Value = value;
    }

    public static implicit operator string(RedirectTargetUrl target) => target.Value;
    public override string ToString() => Value;
}
