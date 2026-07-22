namespace PersonalSite.Api.Application.Skills.GetSkillDetails;

public sealed record GetSkillDetailsResponse
{
    public required int Id { get; init; }

    public required int SkillGroupId { get; init; }

    public required string Name { get; init; }

    public required int DisplayOrder { get; init; }
}