using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Tests.Domain.Skills;

public sealed class SkillNameTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesSkillName()
    {
        var skillName = new SkillName("ASP.NET Core");

        Assert.Equal("ASP.NET Core", skillName.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new SkillName(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var skillName = new SkillName("ASP.NET Core");

        string value = skillName;

        Assert.Equal("ASP.NET Core", value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var skillName = new SkillName("ASP.NET Core");

        Assert.Equal("ASP.NET Core", skillName.ToString());
    }
}