using System.Security.Claims;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Users;


namespace PersonalSite.Api.Endpoints;

public static class ClaimsPrincipalExtensions
{
    public static Actor ToActor(
        this ClaimsPrincipal principal)
    {
        var userIdValue =
            principal.FindFirstValue(
                ClaimTypes.NameIdentifier);

        var roleValue =
            principal.FindFirstValue(
                ClaimTypes.Role);

        if (!int.TryParse(
            userIdValue,
            out var userId))
        {
            throw new InvalidOperationException(
                "Authenticated user has no valid id.");
        }

        if (!Enum.TryParse<UserRole>(
            roleValue,
            out var role))
        {
            throw new InvalidOperationException(
                "Authenticated user has no valid role.");
        }

        return new Actor(
            userId,
            role);
    }
}