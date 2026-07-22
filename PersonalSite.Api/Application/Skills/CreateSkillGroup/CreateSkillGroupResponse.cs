namespace PersonalSite.Api.Application.Skills.CreateSkillGroup;

public sealed record CreateSkillGroupResponse
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required int DisplayOrder { get; init; }
}