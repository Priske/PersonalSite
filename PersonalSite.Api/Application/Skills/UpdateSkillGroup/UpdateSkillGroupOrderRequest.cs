namespace PersonalSite.Api.Application.Skills.UpdateSkillGroup;

public sealed record UpdateSkillGroupOrderRequest(
    IReadOnlyList<int> SkillGroupIds
);