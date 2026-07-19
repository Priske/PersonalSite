using BookTracker.Api.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Security;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Auth.Login;

public class LoginCommandHandler(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    JwtTokenGenerator tokenGenerator) : IHandler
{
    public async Task<LoginResponse?> Execute(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
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
            return null;
        }

        var verification =
            passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return tokenGenerator.Generate(user);
    }
}