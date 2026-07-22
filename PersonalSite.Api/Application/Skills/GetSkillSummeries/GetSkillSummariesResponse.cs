using PersonalSite.Api.Application.Skills.GetSkillSummeries;

namespace PersonalSite.Api.Application.Skills.GetSkillSummaries;

public sealed record GetSkillSummariesResponse
{
    public required IReadOnlyList<SkillSummary> Items { get; init; }
}