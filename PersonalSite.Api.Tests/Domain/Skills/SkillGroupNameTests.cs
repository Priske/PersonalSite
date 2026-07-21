using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Skills;

namespace PersonalSite.Api.Tests.Domain.Skills;

public sealed class SkillGroupNameTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesSkillGroupName()
    {
        var skillGroupName = new SkillGroupName("Backend");

        Assert.Equal("Backend", skillGroupName.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new SkillGroupName(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var skillGroupName = new SkillGroupName("Backend");

        string value = skillGroupName;

        Assert.Equal("Backend", value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var skillGroupName = new SkillGroupName("Backend");

        Assert.Equal("Backend", skillGroupName.ToString());
    }
}