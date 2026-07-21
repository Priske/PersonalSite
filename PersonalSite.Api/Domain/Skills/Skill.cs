
namespace PersonalSite.Api.Domain.Skills;

public sealed class Skill
{
    public int Id { get; init; }

    public int SkillGroupId { get; init; }

    public required SkillName SkillName { get; set; }

    public int DisplayOrder { get; private set; }
}

