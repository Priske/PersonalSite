using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Tests.Domain.Skills;

public sealed class SkillTests
{
    [Fact]
    public void CanCreateSkill()
    {
        var skill = new Skill
        {
            Id = 1,
            SkillGroupId = 2,
            SkillName = new SkillName("ASP.NET Core")
        };

        Assert.Equal(1, skill.Id);
        Assert.Equal(2, skill.SkillGroupId);
        Assert.Equal(new SkillName("ASP.NET Core"), skill.SkillName);
        Assert.Equal(0, skill.DisplayOrder);
    }
}