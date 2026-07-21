using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Skills;

public sealed record SkillGroupName
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    public string Value { get; }

    public SkillGroupName(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Skill group name",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(SkillGroupName value)
        => value.Value;

    public override string ToString()
        => Value;
}