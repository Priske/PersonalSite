namespace PersonalSite.Api.Application.Skills.UpdateSkill;

public sealed class UpdateSkillRequest
{
    public required string Name { get; set; }

    public int DisplayOrder { get; set; }
}