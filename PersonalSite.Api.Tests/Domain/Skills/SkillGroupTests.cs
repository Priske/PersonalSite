using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Tests.Domain.Skills;

public sealed class SkillGroupTests
{
    [Fact]
    public void CanCreateSkillGroup()
    {
        var skillGroup = new SkillGroup
        {
            Id = 1,
            Name = new SkillGroupName("Backend")
        };

        skillGroup.Skills.Add(new Skill
        {
            Id = 1,
            SkillGroupId = 1,
            SkillName = new SkillName("ASP.NET Core")
        });

        Assert.Equal(1, skillGroup.Id);
        Assert.Equal(new SkillGroupName("Backend"), skillGroup.Name);
        Assert.Equal(0, skillGroup.DisplayOrder);

        Assert.Single(skillGroup.Skills);

        var skill = Assert.Single(skillGroup.Skills);
        Assert.Equal(new SkillName("ASP.NET Core"), skill.SkillName);
        Assert.Equal(1, skill.SkillGroupId);
    }
}