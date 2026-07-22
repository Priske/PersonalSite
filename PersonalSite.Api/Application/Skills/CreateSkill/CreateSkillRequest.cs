namespace PersonalSite.Api.Application.Skills.CreateSkill;


public sealed record CreateSkillRequest
{
    public required string Name { get; init; }

    public int DisplayOrder { get; init; }
}
