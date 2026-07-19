using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Security.Password;

public sealed class CompositionPasswordPolicy
    : IPasswordPolicy
{
    public Task ValidateAsync(
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new DomainException("Password is required.");
        }

        if (password.Length < 12)
        {
            throw new DomainException(
                "Password must contain at least 12 characters.");
        }

        if (!password.Any(char.IsUpper))
        {
            throw new DomainException(
                "Password must contain an uppercase letter.");
        }

        if (!password.Any(char.IsLower))
        {
            throw new DomainException(
                "Password must contain a lowercase letter.");
        }

        if (!password.Any(char.IsDigit))
        {
            throw new DomainException(
                "Password must contain a number.");
        }

        if (!password.Any(character => !char.IsLetterOrDigit(character)))
        {
            throw new DomainException(
                "Password must contain a special character.");
        }

        return Task.CompletedTask;
    }
}