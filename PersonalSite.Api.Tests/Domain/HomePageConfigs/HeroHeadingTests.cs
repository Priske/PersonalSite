using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class HeroHeadingTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesHeroHeading()
    {
        var heading =
            new HeroHeading(
                "I build reliable software");

        Assert.Equal(
            "I build reliable software",
            heading.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(
            () =>
                new HeroHeading(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var heading =
            new HeroHeading(
                "I build reliable software");

        string value =
            heading;

        Assert.Equal(
            "I build reliable software",
            value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var heading =
            new HeroHeading(
                "I build reliable software");

        Assert.Equal(
            "I build reliable software",
            heading.ToString());
    }
}