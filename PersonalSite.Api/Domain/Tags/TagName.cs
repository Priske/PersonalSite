
using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Tags;

public sealed record TagName
{
    public const int MinLength = 1;
    public const int MaxLength = 30;

    public string Value { get; }

    public TagName(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Tag Name",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(TagName tagName)
        => tagName.Value;

    public override string ToString()
        => Value;
}