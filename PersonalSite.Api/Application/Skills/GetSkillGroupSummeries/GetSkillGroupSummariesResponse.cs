namespace PersonalSite.Api.Application.Skills.GetSkillGroupSummeries;

public sealed record GetSkillGroupSummariesResponse
{
    public required IReadOnlyList<SkillGroupSummary> Items { get; init; }
}