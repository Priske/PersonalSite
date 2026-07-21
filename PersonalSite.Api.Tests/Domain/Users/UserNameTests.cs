using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Tests.Domain.Users;

public sealed class UserNameTests
{
    [Theory]
    [InlineData("John Smith")]
    [InlineData("Mary-Jane Watson")]
    [InlineData("O'Connor")]
    [InlineData("José García")]
    [InlineData("李小明")]
    public void Constructor_WithValidName_CreatesUserName(string input)
    {
        var userName = new UserName(input);

        Assert.Equal(input, userName.Value);
    }

    [Theory]
    [InlineData("  John Smith  ", "John Smith")]
    [InlineData("John     Smith", "John Smith")]
    [InlineData("  John     Smith  ", "John Smith")]
    [InlineData("John\tSmith", "John Smith")]
    [InlineData("John\nSmith", "John Smith")]
    [InlineData("John\r\nSmith", "John Smith")]
    public void Constructor_NormalizesWhitespace(
        string input,
        string expected)
    {
        var userName = new UserName(input);

        Assert.Equal(expected, userName.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_WithMissingName_ThrowsDomainException(
        string? input)
    {
        var exception = Assert.Throws<DomainException>(
            () => new UserName(input!));

        Assert.Equal("Name is required.", exception.Message);
    }

    [Fact]
    public void Constructor_WithNameShorterThanMinimum_ThrowsDomainException()
    {
        var input = "A";

        var exception = Assert.Throws<DomainException>(
            () => new UserName(input));

        Assert.Equal(
            $"Name must be at least {UserName.MinLength} characters.",
            exception.Message);
    }

    [Fact]
    public void Constructor_WithNameAtMinimumLength_CreatesUserName()
    {
        var input = new string('A', UserName.MinLength);

        var userName = new UserName(input);

        Assert.Equal(input, userName.Value);
    }

    [Fact]
    public void Constructor_WithNameAtMaximumLength_CreatesUserName()
    {
        var input = new string('A', UserName.MaxLength);

        var userName = new UserName(input);

        Assert.Equal(input, userName.Value);
    }

    [Fact]
    public void Constructor_WithNameLongerThanMaximum_ThrowsDomainException()
    {
        var input = new string('A', UserName.MaxLength + 1);

        var exception = Assert.Throws<DomainException>(
            () => new UserName(input));

        Assert.Equal(
            $"Name cannot be longer than {UserName.MaxLength} characters.",
            exception.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("---")]
    [InlineData("'''")]
    [InlineData("123-456")]
    public void Constructor_WithNoLetters_ThrowsDomainException(string input)
    {
        var exception = Assert.Throws<DomainException>(
            () => new UserName(input));

        Assert.Equal(
            "Name must contain at least one letter.",
            exception.Message);
    }

    [Theory]
    [InlineData("John\0Smith")]
    [InlineData("John\bSmith")]
    [InlineData("John\u001BSmith")]
    public void Constructor_WithControlCharacters_ThrowsDomainException(
        string input)
    {
        var exception = Assert.Throws<DomainException>(
            () => new UserName(input));

        Assert.Equal(
            "Name contains invalid characters.",
            exception.Message);
    }

    [Fact]
    public void ToString_ReturnsNormalizedName()
    {
        var userName = new UserName("  John     Smith  ");

        var result = userName.ToString();

        Assert.Equal("John Smith", result);
    }

    [Fact]
    public void ImplicitStringConversion_ReturnsNormalizedName()
    {
        var userName = new UserName("  John     Smith  ");

        string result = userName;

        Assert.Equal("John Smith", result);
    }

    [Fact]
    public void UserNames_WithSameNormalizedValue_AreEqual()
    {
        var first = new UserName("John Smith");
        var second = new UserName("  John     Smith  ");

        Assert.Equal(first, second);
    }

    [Fact]
    public void UserNames_WithDifferentCase_AreNotEqual()
    {
        var first = new UserName("John Smith");
        var second = new UserName("john smith");

        Assert.NotEqual(first, second);
    }
}