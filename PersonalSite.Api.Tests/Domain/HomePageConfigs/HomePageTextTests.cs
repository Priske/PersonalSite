using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class HomePageTextTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesHomePageText()
    {
        var text =
            new HomePageText(
                "View projects",
                "Button Label");

        Assert.Equal(
            "View projects",
            text.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(
            () =>
                new HomePageText(
                    "",
                    "Button Label"));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var text =
            new HomePageText(
                "Contact me",
                "Button Label");

        string value =
            text;

        Assert.Equal(
            "Contact me",
            value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var text =
            new HomePageText(
                "Contact me",
                "Button Label");

        Assert.Equal(
            "Contact me",
            text.ToString());
    }
}