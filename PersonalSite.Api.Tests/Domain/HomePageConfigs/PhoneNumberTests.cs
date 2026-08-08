using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Tests.Domain.HomePageConfigs;

public sealed class PhoneNumberTests
{
    [Theory]
    [InlineData(
        "+32 485 12 34 56",
        "+32485123456")]
    [InlineData(
        "0485 12 34 56",
        "+32485123456")]
    public void Constructor_WithValidPhoneNumber_NormalizesToE164(
        string input,
        string expected)
    {
        var phoneNumber =
            new PhoneNumber(input);

        Assert.Equal(
            expected,
            phoneNumber.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_WithMissingPhoneNumber_ThrowsDomainException(
        string input)
    {
        var exception =
            Assert.Throws<DomainException>(
                () =>
                    new PhoneNumber(input));

        Assert.Equal(
            "Phone number is required.",
            exception.Message);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("not-a-phone-number")]
    public void Constructor_WithInvalidPhoneNumber_ThrowsDomainException(
        string input)
    {
        var exception =
            Assert.Throws<DomainException>(
                () =>
                    new PhoneNumber(input));

        Assert.Equal(
            "Phone number is invalid.",
            exception.Message);
    }

    [Fact]
    public void ImplicitOperator_ReturnsNormalizedValue()
    {
        var phoneNumber =
            new PhoneNumber(
                "0485 12 34 56");

        string value =
            phoneNumber;

        Assert.Equal(
            "+32485123456",
            value);
    }

    [Fact]
    public void ToString_ReturnsNormalizedValue()
    {
        var phoneNumber =
            new PhoneNumber(
                "0485 12 34 56");

        Assert.Equal(
            "+32485123456",
            phoneNumber.ToString());
    }
}