using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Common;

public sealed record Url
{
    public string Value { get; }

    public Url(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("URL is required.");
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            throw new DomainException("URL is invalid.");
        }

        if (uri.Scheme != Uri.UriSchemeHttp &&
            uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new DomainException("URL must use HTTP or HTTPS.");
        }

        Value = uri.ToString();
    }

    public static implicit operator string(Url url)
        => url.Value;

    public override string ToString()
        => Value;
}