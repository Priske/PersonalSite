namespace PersonalSite.Api.Application.Skills.CreateSkill;

public class CreateSkillResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int SkillGroupId { get; set; }

    public int DisplayOrder { get; set; }

}


