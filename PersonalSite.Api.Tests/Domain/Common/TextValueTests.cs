using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Tests.Domain.Common;

public sealed class TextValueTests
{
    [Fact]
    public void Create_WithValidValue_ReturnsValue()
    {
        var result = TextValue.Create(
            "John Doe",
            "Name",
            minLength: 2,
            maxLength: 100);

        Assert.Equal("John Doe", result);
    }

    [Fact]
    public void Create_WithSurroundingWhitespace_TrimsWhitespace()
    {
        var result = TextValue.Create(
            "   John Doe   ",
            "Name",
            minLength: 2,
            maxLength: 100);

        Assert.Equal("John Doe", result);
    }

    [Fact]
    public void Create_WithRepeatedWhitespace_CollapsesWhitespace()
    {
        var result = TextValue.Create(
            "John     Doe",
            "Name",
            minLength: 2,
            maxLength: 100);

        Assert.Equal("John Doe", result);
    }

    [Fact]
    public void Create_WithTabsAndNewLines_NormalizesWhitespace()
    {
        var result = TextValue.Create(
            "John\t\nDoe",
            "Name",
            minLength: 2,
            maxLength: 100);

        Assert.Equal("John Doe", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("    ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithMissingValue_ThrowsDomainException(string? value)
    {
        var exception = Assert.Throws<DomainException>(() =>
            TextValue.Create(
                value,
                "Name",
                minLength: 2,
                maxLength: 100));

        Assert.Equal("Name is required.", exception.Message);
    }

    [Fact]
    public void Create_WithValueShorterThanMinimum_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            TextValue.Create(
                "A",
                "Name",
                minLength: 2,
                maxLength: 100));

        Assert.Equal(
            "Name must be at least 2 characters.",
            exception.Message);
    }

    [Fact]
    public void Create_WithValueEqualToMinimum_ReturnsValue()
    {
        var result = TextValue.Create(
            "Ab",
            "Name",
            minLength: 2,
            maxLength: 100);

        Assert.Equal("Ab", result);
    }

    [Fact]
    public void Create_WithValueLongerThanMaximum_ThrowsDomainException()
    {
        var value = new string('A', 101);

        var exception = Assert.Throws<DomainException>(() =>
            TextValue.Create(
                value,
                "Name",
                minLength: 2,
                maxLength: 100));

        Assert.Equal(
            "Name cannot be longer than 100 characters.",
            exception.Message);
    }

    [Fact]
    public void Create_WithValueEqualToMaximum_ReturnsValue()
    {
        var value = new string('A', 100);

        var result = TextValue.Create(
            value,
            "Name",
            minLength: 2,
            maxLength: 100);

        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData("John\u0000Doe")] // Null
    [InlineData("John\u0001Doe")] // Start of Heading
    [InlineData("John\u0002Doe")] // Start of Text
    [InlineData("John\u0003Doe")] // End of Text
    [InlineData("John\u0004Doe")] // End of Transmission
    [InlineData("John\u0007Doe")] // Bell
    [InlineData("John\u0008Doe")] // Backspace
    [InlineData("John\u001BDoe")] // Escape
    [InlineData("John\u007FDoe")] // Delete
    public void Create_WithControlCharacter_ThrowsDomainException(string value)
    {
        var exception = Assert.Throws<DomainException>(() =>
            TextValue.Create(
                value,
                "Name",
                minLength: 2,
                maxLength: 100));

        Assert.Equal(
            "Name contains invalid characters.",
            exception.Message);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("---")]
    [InlineData("123-456")]
    public void Create_WithoutAnyLetters_ThrowsDomainException(string value)
    {
        var exception = Assert.Throws<DomainException>(() =>
            TextValue.Create(
                value,
                "Name",
                minLength: 2,
                maxLength: 100));

        Assert.Equal(
            "Name must contain at least one letter.",
            exception.Message);
    }

    [Theory]
    [InlineData("C#")]
    [InlineData("C++")]
    [InlineData(".NET")]
    [InlineData("Node.js")]
    public void Create_WithLettersAndSymbols_ReturnsValue(string value)
    {
        var result = TextValue.Create(
            value,
            "Skill name",
            minLength: 1,
            maxLength: 100);

        Assert.Equal(value, result);
    }

    [Fact]
    public void Create_UsesProvidedFieldNameInErrorMessage()
    {
        var exception = Assert.Throws<DomainException>(() =>
            TextValue.Create(
                null,
                "Project title",
                minLength: 2,
                maxLength: 100));

        Assert.Equal(
            "Project title is required.",
            exception.Message);
    }
}