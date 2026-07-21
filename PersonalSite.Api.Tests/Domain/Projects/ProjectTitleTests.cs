using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Tests.Domain.Projects;
// Most Gets Tested in ValueText
// This is Added incase functionality diverges
public sealed class ProjectTitleTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesProjectTitle()
    {
        var title = new ProjectTitle("Personal Site");

        Assert.Equal("Personal Site", title.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new ProjectTitle(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var title = new ProjectTitle("Personal Site");

        string value = title;

        Assert.Equal("Personal Site", value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var title = new ProjectTitle("Personal Site");

        Assert.Equal("Personal Site", title.ToString());
    }
}