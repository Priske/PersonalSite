using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class SectionNumberTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("01")]
    [InlineData("123")]
    [InlineData("0002")]
    public void Constructor_WithValidValue_CreatesSectionNumber(
        string input)
    {
        var sectionNumber =
            new SectionNumber(input);

        Assert.Equal(
            input,
            sectionNumber.Value);
    }

    [Fact]
    public void Constructor_WithSurroundingWhitespace_TrimsValue()
    {
        var sectionNumber =
            new SectionNumber(
                "   02   ");

        Assert.Equal(
            "02",
            sectionNumber.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_WithMissingValue_ThrowsDomainException(
        string? input)
    {
        var exception =
            Assert.Throws<DomainException>(
                () =>
                    new SectionNumber(input));

        Assert.Equal(
            "Contact Section Number is required.",
            exception.Message);
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("1A")]
    [InlineData("02.")]
    [InlineData("-2")]
    public void Constructor_WithNonNumericValue_ThrowsDomainException(
        string input)
    {
        var exception =
            Assert.Throws<DomainException>(
                () =>
                    new SectionNumber(input));

        Assert.Equal(
            "Contact Section Number may only contain numbers.",
            exception.Message);
    }

    [Fact]
    public void Constructor_WithValueLongerThanMaximum_ThrowsDomainException()
    {
        var value =
            new string(
                '1',
                SectionNumber.MaxLength + 1);

        var exception =
            Assert.Throws<DomainException>(
                () =>
                    new SectionNumber(value));

        Assert.Equal(
            $"Contact Section Number cannot be longer than {SectionNumber.MaxLength} characters.",
            exception.Message);
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var sectionNumber =
            new SectionNumber("02");

        string value =
            sectionNumber;

        Assert.Equal(
            "02",
            value);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var sectionNumber =
            new SectionNumber("02");

        Assert.Equal(
            "02",
            sectionNumber.ToString());
    }
}