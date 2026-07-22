using PersonalSite.Api.Application.Skills.GetSkillSummeries;

namespace PersonalSite.Api.Application.Skills.GetSkillGroupDetails;

public sealed record GetSkillGroupDetailsResponse
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required int DisplayOrder { get; init; }

    public required IReadOnlyList<SkillSummary> Skills { get; init; }
}