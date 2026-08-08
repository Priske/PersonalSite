using System.Security.Claims;
using PersonalSite.Api.Application.Auth.GetCurrentUser;
using PersonalSite.Api.Application.Auth.Login;


namespace PersonalSite.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", Login);

        app.MapGet("/auth/me", GetCurrentUser)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        LoginCommandHandler handler)
    {
        var response = await handler.Execute(request);

        if (response is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCurrentUser(
     ClaimsPrincipal principal,
     GetCurrentUserQueryHandler handler,
      CancellationToken cancellationToken)
    {
        var idValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(idValue, out var id))
        {
            return Results.Unauthorized();
        }

        var user = await handler.Execute(id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }
}