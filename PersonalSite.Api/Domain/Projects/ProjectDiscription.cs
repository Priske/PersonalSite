using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Projects;

public sealed record ProjectDescription
{
    public const int MinLength = 2;
    public const int MaxLength = 500;

    public string Value { get; }

    public ProjectDescription(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Project description",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(ProjectDescription description)
        => description.Value;

    public override string ToString()
        => Value;
}