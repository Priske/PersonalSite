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
        app.MapGet("/home-official-page-config", GetOfficalHomePageDetails);
        app.MapGet("/home-demo-page-config", GetDemoHomePageDetails)
            .RequireAuthorization();
        app.MapPut("/home-official-page-config", UpdateOfficialHomePageConfig)
            .RequireAuthorization();
        app.MapPut("/home-demo-page-config", UpdateDemoHomePageConfig)
           .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetOfficalHomePageDetails(
        GetOfficialHomePageDetailsQueryHandler queryHandler,
        CancellationToken cancellationToken)
    {
        var config = await queryHandler.Execute(cancellationToken);

        return config is null
            ? Results.NotFound()
            : Results.Ok(config);
    }

    private static async Task<IResult> GetDemoHomePageDetails(
       ClaimsPrincipal principal,
       GetDemoHomePageDetailsQueryHandler queryHandler,
       CancellationToken cancellationToken)
    {
        var actor = principal.ToActor();

        var config = await queryHandler.Execute(
            actor,
            cancellationToken);

        return config is null
            ? Results.NotFound()
            : Results.Ok(config);
    }
    private static async Task<IResult> UpdateOfficialHomePageConfig(
        ClaimsPrincipal principal,
        UpdateHomePageConfigRequest request,
        UpdateOfficialHomePageConfigCommandHandler commandHandler,
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

    private static async Task<IResult> UpdateDemoHomePageConfig(
    ClaimsPrincipal principal,
    UpdateHomePageConfigRequest request,
    UpdateDemoHomePageConfigCommandHandler commandHandler,
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