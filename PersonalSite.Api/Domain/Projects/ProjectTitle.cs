
using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Projects;

public sealed record ProjectTitle
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public ProjectTitle(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Project title",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(ProjectTitle title)
        => title.Value;

    public override string ToString()
        => Value;
}