using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class EmailAddressTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("first.last@example.com")]
    [InlineData("user+tag@example.com")]
    [InlineData("user123@subdomain.example.com")]
    public void Constructor_WithValidEmail_CreatesEmailAddress(
        string input)
    {
        var email =
            new EmailAddress(input);

        Assert.Equal(
            input,
            email.Value);
    }

    [Theory]
    [InlineData(
        " TEST@EXAMPLE.COM ",
        "test@example.com")]
    [InlineData(
        "\tUser@Example.com\t",
        "user@example.com")]
    [InlineData(
        "MixedCase@DOMAIN.COM",
        "mixedcase@domain.com")]
    public void Constructor_NormalizesEmail(
        string input,
        string expected)
    {
        var email =
            new EmailAddress(input);

        Assert.Equal(
            expected,
            email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Constructor_WithMissingEmail_ThrowsDomainException(
        string input)
    {
        var exception =
            Assert.Throws<DomainException>(
                () =>
                    new EmailAddress(input));

        Assert.Equal(
            "Email is required.",
            exception.Message);
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
        var exception =
            Assert.Throws<DomainException>(
                () =>
                    new EmailAddress(input));

        Assert.Equal(
            "Email is not valid.",
            exception.Message);
    }

    [Fact]
    public void Constructor_WithEmailLongerThanMaximum_ThrowsDomainException()
    {
        var localPart =
            new string(
                'a',
                EmailAddress.MaxLength);

        var input =
            $"{localPart}@example.com";

        var exception =
            Assert.Throws<DomainException>(
                () =>
                    new EmailAddress(input));

        Assert.Equal(
            $"Email cannot be longer than {EmailAddress.MaxLength} characters.",
            exception.Message);
    }

    [Fact]
    public void ImplicitOperator_ReturnsNormalizedEmail()
    {
        var email =
            new EmailAddress(
                " Test@Example.com ");

        string value =
            email;

        Assert.Equal(
            "test@example.com",
            value);
    }

    [Fact]
    public void ToString_ReturnsNormalizedEmail()
    {
        var email =
            new EmailAddress(
                " Test@Example.com ");

        Assert.Equal(
            "test@example.com",
            email.ToString());
    }
}