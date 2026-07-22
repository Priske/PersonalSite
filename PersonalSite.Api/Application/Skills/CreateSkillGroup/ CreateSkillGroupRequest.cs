namespace PersonalSite.Api.Application.Skills.CreateSkillGroup;

public sealed record CreateSkillGroupRequest
{
    public required string Name { get; init; }

    public int DisplayOrder { get; init; }
}