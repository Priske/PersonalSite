using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Tests.Domain.Users;

public class UserEmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("first.last@example.com")]
    [InlineData("user+tag@example.com")]
    [InlineData("user123@subdomain.example.com")]
    public void Constructor_WithValidEmail_CreatesUserEmail(string input)
    {
        var email = new UserEmail(input);

        Assert.Equal(input, email.Value);
    }
    [Theory]
    [InlineData(" TEST@EXAMPLE.COM ", "test@example.com")]
    [InlineData("\tUser@Example.com\t", "user@example.com")]
    [InlineData("MixedCase@DOMAIN.COM", "mixedcase@domain.com")]
    public void Constructor_NormalizesEmail(
          string input,
          string expected)
    {
        var email = new UserEmail(input);

        Assert.Equal(expected, email.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_WithMissingEmail_ThrowsDomainException(
        string? input)
    {
        var exception = Assert.Throws<DomainException>(
            () => new UserEmail(input!));

        Assert.Equal("User email is required.", exception.Message);
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@@example.com")]
    [InlineData("user example@example.com")]
    public void Constructor_WithInvalidEmail_ThrowsDomainException(
        string input)
    {
        var exception = Assert.Throws<DomainException>(
            () => new UserEmail(input));

        Assert.Equal("User email is not valid.", exception.Message);
    }

    [Fact]
    public void ToString_ReturnsNormalizedEmail()
    {
        var email = new UserEmail(" Test@Example.com ");

        Assert.Equal("test@example.com", email.ToString());
    }

    [Fact]
    public void ImplicitStringConversion_ReturnsNormalizedEmail()
    {
        var email = new UserEmail(" Test@Example.com ");

        string value = email;

        Assert.Equal("test@example.com", value);
    }

    [Fact]
    public void Emails_WithSameNormalizedValue_AreEqual()
    {
        var first = new UserEmail("Test@Example.com");
        var second = new UserEmail(" test@example.com ");

        Assert.Equal(first, second);
    }

    [Fact]
    public void Constructor_WithEmailLongerThanMaximum_ThrowsDomainException()
    {
        var localPart = new string('a', EmailValue.MaxLength);
        var input = $"{localPart}@example.com";

        var exception = Assert.Throws<DomainException>(
            () => new UserEmail(input));

        Assert.Equal(
            $"User email cannot be longer than {EmailValue.MaxLength} characters.",
            exception.Message);
    }


}
