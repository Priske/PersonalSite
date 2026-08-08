using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class ContactHeadingTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesContactHeading()
    {
        var heading =
            new ContactHeading(
                "Interested in working together?");

        Assert.Equal(
            "Interested in working together?",
            heading.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(
            () =>
                new ContactHeading(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var heading =
            new ContactHeading(
                "Let's talk");

        string value =
            heading;

        Assert.Equal(
            "Let's talk",
            value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var heading =
            new ContactHeading(
                "Let's talk");

        Assert.Equal(
            "Let's talk",
            heading.ToString());
    }
}