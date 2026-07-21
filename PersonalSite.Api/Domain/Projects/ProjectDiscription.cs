
using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Projects;

public sealed record ProjectDiscription
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public ProjectDiscription(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Project discription",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(ProjectDiscription discription)
        => discription.Value;

    public override string ToString()
        => Value;
}