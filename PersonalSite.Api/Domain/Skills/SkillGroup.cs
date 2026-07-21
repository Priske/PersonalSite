namespace PersonalSite.Api.Domain.Skills;

public sealed class SkillGroup
{
    public int Id { get; init; }

    public required SkillGroupName Name { get; set; }

    public int DisplayOrder { get; private set; }

    public ICollection<Skill> Skills { get; } = [];
}