namespace PersonalSite.Api.Domain.Skills;

public sealed class SkillGroup : SiteContent
{
    public int Id { get; init; }

    public required SkillGroupName Name { get; set; }

    public int DisplayOrder { get; set; }

    public ICollection<Skill> Skills { get; } = [];
}