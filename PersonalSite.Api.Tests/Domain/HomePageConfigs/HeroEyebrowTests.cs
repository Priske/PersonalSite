using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class HeroEyebrowTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesHeroEyebrow()
    {
        var eyebrow =
            new HeroEyebrow(
                "Hello there");

        Assert.Equal(
            "Hello there",
            eyebrow.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(
            () =>
                new HeroEyebrow(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var eyebrow =
            new HeroEyebrow(
                "Hello there");

        string value =
            eyebrow;

        Assert.Equal(
            "Hello there",
            value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var eyebrow =
            new HeroEyebrow(
                "Hello there");

        Assert.Equal(
            "Hello there",
            eyebrow.ToString());
    }
}