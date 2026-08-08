using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class HeroSummaryTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesHeroSummary()
    {
        var summary =
            new HeroSummary(
                "I create reliable software applications.");

        Assert.Equal(
            "I create reliable software applications.",
            summary.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(
            () =>
                new HeroSummary(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var summary =
            new HeroSummary(
                "I create reliable software applications.");

        string value =
            summary;

        Assert.Equal(
            "I create reliable software applications.",
            value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var summary =
            new HeroSummary(
                "I create reliable software applications.");

        Assert.Equal(
            "I create reliable software applications.",
            summary.ToString());
    }
}