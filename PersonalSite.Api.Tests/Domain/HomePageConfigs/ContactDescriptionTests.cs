using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class ContactDescriptionTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesContactDescription()
    {
        var description =
            new ContactDescription(
                "Feel free to get in touch.");

        Assert.Equal(
            "Feel free to get in touch.",
            description.Value);
    }

    [Fact]
    public void Constructor_WithInvalidValue_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(
            () =>
                new ContactDescription(""));
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var description =
            new ContactDescription(
                "Feel free to get in touch.");

        string value =
            description;

        Assert.Equal(
            "Feel free to get in touch.",
            value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var description =
            new ContactDescription(
                "Feel free to get in touch.");

        Assert.Equal(
            "Feel free to get in touch.",
            description.ToString());
    }
}