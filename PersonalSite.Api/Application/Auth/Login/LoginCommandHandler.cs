using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Security;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Auth.Login;

public class LoginCommandHandler(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    JwtTokenGenerator tokenGenerator,
    ILogger<LoginCommandHandler> logger) : IHandler
{
    public async Task<LoginResponse?> Execute(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogWarning("Login failed because credentials were missing");
            return null;
        }

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var user =
            await dbContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(user =>
                    (string)user.Email == email);

        if (user is null)
        {
            logger.LogWarning("Login failed for unknown email {Email}", email);
            return null;
        }

        var verification =
            passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Login failed for user {UserId}", user.Id);
            return null;
        }
        logger.LogInformation("User {UserId} logged in", user.Id);

        return tokenGenerator.Generate(user);
    }
}