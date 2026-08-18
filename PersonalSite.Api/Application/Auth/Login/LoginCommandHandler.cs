using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Security;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Auth.Login;

public class LoginCommandHandler(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    JwtTokenGenerator tokenGenerator,
    ILogger<LoginCommandHandler> logger,
    ActivityTracker activityTracker) : IHandler
{
    public async Task<LoginResponse?> Execute(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogWarning(
                "Login failed because credentials were missing");

            await activityTracker.TrackAsync(
                ActivityType.LoginFailed,
                null,
                metadata =>
                {
                    metadata.Add(
                        "reason",
                        new StringMetadataValue("missing_credentials"));
                },
                cancellationToken);

            return null;
        }

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var user =
            await dbContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    user => (string)user.Email == email,
                    cancellationToken);

        if (user is null)
        {
            logger.LogWarning(
                "Login failed for unknown user");

            await activityTracker.TrackAsync(
                ActivityType.LoginFailed,
                null,
                metadata =>
                {
                    metadata.Add(
                        "reason",
                        new StringMetadataValue("unknown_email"));

                    metadata.Add(
                        "attempted_email",
                        new StringMetadataValue(email));
                },
                cancellationToken);

            return null;
        }

        var verification =
            passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            logger.LogWarning(
                "Login failed for user {UserId}",
                user.Id);

            await activityTracker.TrackAsync(
                ActivityType.LoginFailed,
                user.Id,
                metadata =>
                {
                    metadata.Add(
                        "reason",
                        new StringMetadataValue("incorrect_password"));
                },
                cancellationToken);

            return null;
        }

        logger.LogInformation(
            "User {UserId} logged in",
            user.Id);

        await activityTracker.TrackAsync(
            ActivityType.Login,
            user.Id,
            null,
            cancellationToken);

        return tokenGenerator.Generate(user);
    }
}