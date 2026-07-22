namespace PersonalSite.Api.Application.Skills.UpdateSkillGroup;

public sealed class UpdateSkillGroupRequest
{
    public required string Name { get; set; }

    public int DisplayOrder { get; set; }
}