using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Tests.Domain.Projects;
// Most Gets Tested in ValueText
// This is Added incase functionality diverges
public sealed class ProjectDiscriptionTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesProjectDiscription()
    {
        var description = new ProjectDiscription("A personal portfolio website.");

        Assert.Equal("A personal portfolio website.", description.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new ProjectDiscription(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var description = new ProjectDiscription("A personal portfolio website.");

        string value = description;

        Assert.Equal("A personal portfolio website.", value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var description = new ProjectDiscription("A personal portfolio website.");

        Assert.Equal("A personal portfolio website.", description.ToString());
    }
}