using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Security.Password;

namespace PersonalSite.Api.Tests.IntegrationTests.Security.PasswordTests;

public sealed class PassphrasePasswordPolicyTests
{
    [Theory]
    [InlineData("correct horse battery staple")]
    [InlineData("My dog eats blue umbrellas")]
    [InlineData("coffee-rain-window-bicycle")]
    [InlineData("This password has spaces!")]
    [InlineData("José drinks koffie every morning")]
    public async Task ValidateAsync_WithValidPassword_DoesNotThrow(
        string password)
    {
        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

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
        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal("Password is required.", exception.Message);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("Password1!")]
    [InlineData("short password")]
    [InlineData("12345678901234")]
    public async Task ValidateAsync_WithPasswordBelowMinimumLength_ThrowsDomainException(
        string password)
    {
        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            $"Password must contain at least {PassphrasePasswordPolicy.MinLength} characters.",
            exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_WithPasswordAtMinimumLength_DoesNotThrow()
    {
        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        var password = new string(
            'a',
            PassphrasePasswordPolicy.MinLength);

        var exception = await Record.ExceptionAsync(
            () => policy.ValidateAsync(password));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_WithPasswordAtMaximumLength_DoesNotThrow()
    {
        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        var password = new string(
            'a',
            PassphrasePasswordPolicy.MaxLength);

        var exception = await Record.ExceptionAsync(
            () => policy.ValidateAsync(password));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_WithPasswordAboveMaximumLength_ThrowsDomainException()
    {
        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        var password = new string(
            'a',
            PassphrasePasswordPolicy.MaxLength + 1);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            $"Password cannot contain more than {PassphrasePasswordPolicy.MaxLength} characters.",
            exception.Message);
    }

    [Theory]
    [InlineData("passwordpassword")]
    [InlineData("Password123456!")]
    [InlineData("qwertyqwertyqwerty")]
    [InlineData("letmeinletmein123")]
    public async Task ValidateAsync_WithCompromisedPassword_ThrowsDomainException(
        string password)
    {
        var checker = new FakeCompromisedPasswordChecker([password]);
        var policy = new PassphrasePasswordPolicy(checker);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            "This password is too common or has appeared in a known data breach.",
            exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_WithLongButCompromisedPassword_RejectsPassword()
    {
        var password = "passwordpasswordpassword";

        var checker = new FakeCompromisedPasswordChecker([password]);
        var policy = new PassphrasePasswordPolicy(checker);

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync(password));

        Assert.Equal(
            "This password is too common or has appeared in a known data breach.",
            exception.Message);
    }

    [Fact]
    public async Task ValidateAsync_WithSpaces_DoesNotTrimPassword()
    {
        var password = "  strong password phrase  ";

        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        await policy.ValidateAsync(password);

        Assert.Equal(password, checker.LastCheckedPassword);
    }

    [Fact]
    public async Task ValidateAsync_WithUppercaseAndLowercase_PreservesPasswordExactly()
    {
        var password = "My Unique Password Phrase";

        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        await policy.ValidateAsync(password);

        Assert.Equal(password, checker.LastCheckedPassword);
    }

    [Fact]
    public async Task ValidateAsync_WithValidPassword_ChecksCompromisedPasswordService()
    {
        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        var password = "a sufficiently long password";

        await policy.ValidateAsync(password);

        Assert.True(checker.WasCalled);
        Assert.Equal(password, checker.LastCheckedPassword);
    }

    [Fact]
    public async Task ValidateAsync_WithTooShortPassword_DoesNotCallCompromisedChecker()
    {
        var checker = new FakeCompromisedPasswordChecker();
        var policy = new PassphrasePasswordPolicy(checker);

        await Assert.ThrowsAsync<DomainException>(
            () => policy.ValidateAsync("short"));

        Assert.False(checker.WasCalled);
    }
}