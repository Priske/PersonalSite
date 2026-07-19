using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Security.Password;

public sealed class PassphrasePasswordPolicy(
    ICompromisedPasswordChecker compromisedPasswordChecker)
    : IPasswordPolicy
{
    public const int MinLength = 15;
    public const int MaxLength = 128;

    public async Task ValidateAsync(
        string? password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new DomainException("Password is required.");
        }

        if (password.Length < MinLength)
        {
            throw new DomainException(
                $"Password must contain at least {MinLength} characters.");
        }

        if (password.Length > MaxLength)
        {
            throw new DomainException(
                $"Password cannot contain more than {MaxLength} characters.");
        }

        if (await compromisedPasswordChecker.IsCompromisedAsync(
                password,
                cancellationToken))
        {
            throw new DomainException(
                "This password is too common or has appeared in a known data breach.");
        }
    }
}