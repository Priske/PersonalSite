using System.Security.Claims;
using PersonalSite.Api.Application.HomePageConfigs.GetHomePageDetails;
using PersonalSite.Api.Application.HomePageConfigs.UpdateConfig;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.HomePageConfigs;

public static class HomePageConfigEndpoints
{
    public static IEndpointRouteBuilder MapHomePageEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/home-page-config", GetHomePageDetails);

        app.MapPut("/home-page-config", UpdateHomePageConfig)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetHomePageDetails(
        GetHomePageDetailsQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        var config = await queryHandler.Execute(cancellationToken);

        return config is null
            ? Results.NotFound()
            : Results.Ok(config);
    }

    private static async Task<IResult> UpdateHomePageConfig(
        ClaimsPrincipal principal,
        UpdateHomePageConfigRequest request,
        UpdateHomePageConfigCommandHandler commandHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            var updated = await commandHandler.Execute(
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
            return Results.BadRequest(new
            {
                error = exception.Message
            });
        }
    }
}