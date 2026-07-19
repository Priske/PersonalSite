using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Security.Password;

namespace PersonalSite.Api.Tests.IntegrationTests.Security.PasswordTests;

public sealed class CompositionPasswordPolicyTests
{
    [Theory]
    [InlineData("StrongPass1!")]
    [InlineData("AnotherGood2@")]
    [InlineData("LongerPassword9#")]
    [InlineData("CaféPassword7!")]
    public async Task ValidateAsync_WithValidPassword_DoesNotThrow(
        string password)
    {
        var policy = new CompositionPasswordPolicy();

        var exception = await Record.ExceptionAsync(
            () => policy.ValidateAsync(password));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ValidateAsync_WithMissingPassword_ThrowsDomainException(
        string? password)
    {
        var policy = new CompositionPasswordPolicy();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password!));

        Assert.Equal("Password is required.", exception.Message);
    }

    [Theory]
    [InlineData("Short1!")]
    [InlineData("Abcdefgh1!")]
    [InlineData("Password1!")]
    public async Task ValidateAsync_WithPasswordBelowMinimumLength_ThrowsDomainException(
        string password)
    {
        var policy = new CompositionPasswordPolicy();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            "Password must contain at least 12 characters.",
            exception.Message);
    }

    [Theory]
    [InlineData("lowercase123!")]
    [InlineData("password123!")]
    [InlineData("alllowercase7#")]
    public async Task ValidateAsync_WithoutUppercaseLetter_ThrowsDomainException(
        string password)
    {
        var policy = new CompositionPasswordPolicy();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            "Password must contain an uppercase letter.",
            exception.Message);
    }

    [Theory]
    [InlineData("UPPERCASE123!")]
    [InlineData("PASSWORD123!")]
    [InlineData("ALLUPPERCASE7#")]
    public async Task ValidateAsync_WithoutLowercaseLetter_ThrowsDomainException(
        string password)
    {
        var policy = new CompositionPasswordPolicy();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            "Password must contain a lowercase letter.",
            exception.Message);
    }

    [Theory]
    [InlineData("PasswordOnly!")]
    [InlineData("NoDigitsHere#")]
    [InlineData("StillNoNumber@")]
    public async Task ValidateAsync_WithoutNumber_ThrowsDomainException(
        string password)
    {
        var policy = new CompositionPasswordPolicy();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            "Password must contain a number.",
            exception.Message);
    }

    [Theory]
    [InlineData("Password1234")]
    [InlineData("NoSpecialChar7")]
    [InlineData("StillMissing9")]
    public async Task ValidateAsync_WithoutSpecialCharacter_ThrowsDomainException(
        string password)
    {
        var policy = new CompositionPasswordPolicy();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            "Password must contain a special character.",
            exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_WithExactlyTwelveCharacters_DoesNotThrow()
    {
        var policy = new CompositionPasswordPolicy();

        var password = "StrongPass1!";

        Assert.Equal(12, password.Length);

        var exception = await Record.ExceptionAsync(
            () => policy.ValidateAsync(password));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_WithSpaces_CountsSpaceAsSpecialCharacter()
    {
        var policy = new CompositionPasswordPolicy();

        var password = "Strong Pass12";

        var exception = await Record.ExceptionAsync(
            () => policy.ValidateAsync(password));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_WithUnicodeLetters_DoesNotThrow()
    {
        var policy = new CompositionPasswordPolicy();

        var password = "ÉénSterkPwd7!";

        var exception = await Record.ExceptionAsync(
            () => policy.ValidateAsync(password));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_WhenMultipleRulesFail_ReturnsFirstValidationError()
    {
        var policy = new CompositionPasswordPolicy();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync("short"));

        Assert.Equal(
            "Password must contain at least 12 characters.",
            exception.Message);
    }
}