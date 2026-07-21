using PersonalSite.Api.Domain.Common;

namespace PersonalSite.Api.Domain.Skills;

public sealed record SkillName
{
    public const int MinLength = 1;
    public const int MaxLength = 100;

    public string Value { get; }

    public SkillName(string? value)
    {
        Value = TextValue.Create(
            value,
            fieldName: "Skill name",
            minLength: MinLength,
            maxLength: MaxLength);
    }

    public static implicit operator string(SkillName value)
        => value.Value;

    public override string ToString()
        => Value;
}