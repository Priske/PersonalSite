using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Tests.Domain.Tags;

public sealed class TagNameTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesTagName()
    {
        var tagName = new TagName("ASP.NET Core");

        Assert.Equal("ASP.NET Core", tagName.Value);
    }

    [Fact]
    public void Constructor_WithSurroundingWhitespace_TrimsValue()
    {
        var tagName = new TagName("   React   ");

        Assert.Equal("React", tagName.Value);
    }

    [Fact]
    public void Constructor_WithMissingValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(
            () => new TagName(""));
    }

    [Fact]
    public void Constructor_WithValueLongerThanMaximum_ThrowsDomainException()
    {
        var value =
            new string(
                'A',
                TagName.MaxLength + 1);

        Assert.Throws<DomainException>(
            () => new TagName(value));
    }

    [Fact]
    public void Constructor_WithValueEqualToMaximum_CreatesTagName()
    {
        var value =
            new string(
                'A',
                TagName.MaxLength);

        var tagName =
            new TagName(value);

        Assert.Equal(
            value,
            tagName.Value);
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var tagName =
            new TagName("C#");

        string value =
            tagName;

        Assert.Equal(
            "C#",
            value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var tagName =
            new TagName("C#");

        Assert.Equal(
            "C#",
            tagName.ToString());
    }
}