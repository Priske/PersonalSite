namespace PersonalSite.Api.Application.Skills.UpdateSkill;

public sealed record UpdateSkillOrderRequest
{
    public required IReadOnlyList<int> SkillIds { get; init; }
}