
using System.Security.Claims;
using PersonalSite.Api.Application.HomePageConfigs.UpdateConfig;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.HomePageConfigs;

public static class HomePageConfigEndpoints
{
    public static IEndpointRouteBuilder MapHomePageEndpoints(
        this IEndpointRouteBuilder app)
    {
        // app.MapGet("/home-page-config/", GetHomePageDetails);

        app.MapPut("/home-page-config/", UpdateHomePageConfig)
            .RequireAuthorization();

        return app;
    }



    public static async Task<IResult> UpdateHomePageConfig(
        ClaimsPrincipal principal,
        UpdateHomePageConfigRequest request,
        UpdateHomePageConfigCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            var updated = await handler.Execute(
                actor,
                request,
                cancellationToken);

            return updated
                ? Results.NoContent()
                : Results.NotFound();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(
                new { error = exception.Message });
        }
    }
}