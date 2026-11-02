using System.Text.RegularExpressions;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Redirects;

public sealed partial record RedirectSlug
{
    public const int MaxLength = 80;
    public const string ValidationMessage = "Use up to 80 lowercase letters, numbers and single separating hyphens.";

    public string Value { get; }

    public RedirectSlug(string? value)
    {
        if (!IsValid(value)) throw new DomainException(ValidationMessage);
        Value = value!;
    }

    public static bool IsValid(string? value) =>
        value is { Length: > 0 and <= MaxLength } && SlugPattern().IsMatch(value);

    [GeneratedRegex(@"\A[a-z0-9]+(?:-[a-z0-9]+)*\z", RegexOptions.CultureInvariant)]
    private static partial Regex SlugPattern();

    public static implicit operator string(RedirectSlug slug) => slug.Value;
    public override string ToString() => Value;
}
