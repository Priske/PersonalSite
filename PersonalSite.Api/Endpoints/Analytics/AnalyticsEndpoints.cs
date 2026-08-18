
using System.Security.Claims;
using PersonalSite.Api.Application.Analytics.GetCreateUserActivity;
using PersonalSite.Api.Application.Analytics.GetCreateUserRequest.cs;
using PersonalSite.Api.Application.Analytics.GetDeleteUserActivity;
using PersonalSite.Api.Application.Analytics.GetLoginActivity;
using PersonalSite.Api.Application.Analytics.GetReferrerActivity;
using PersonalSite.Api.Application.Analytics.TrackActivity;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Endpoints.Analytics;

public static class AnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/analytics", TrackActivity);
        app.MapGet("/analytics/referrers", GetReferrerActivity);
        app.MapGet("/analytics/login", GetLoginActivity).RequireAuthorization();
        app.MapGet("/analytics/create-users", GetCreateUserAnalytics).RequireAuthorization();
        app.MapGet("/analytics/delete-users", GetDeleteUsersAnalytics).RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetDeleteUsersAnalytics(
        [AsParameters] GetDeleteUserAnalyticsRequest request,
        ClaimsPrincipal principal,
        DeleteUserActivityCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            var response = await handler.ExecuteAsync(
                actor,
                request,
                cancellationToken);

            return Results.Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
    }
    private static async Task<IResult> GetCreateUserAnalytics(
        [AsParameters] GetCreateUserAnalyticsRequest request,
        ClaimsPrincipal principal,
        CreateUserActivityCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            var response = await handler.ExecuteAsync(
                actor,
                request,
                cancellationToken);

            return Results.Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
    }
    private static async Task<IResult> GetReferrerActivity(
        [AsParameters] GetReferrerAnalyticsRequest request,
        ReferrerActivityCommandHandler handler,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var response = await handler.ExecuteAsync(request, cancellationToken);
            return Results.Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }

    }

    private static async Task<IResult> GetLoginActivity(
        [AsParameters] GetLoginAnalyticsRequest request,
        ClaimsPrincipal principal,
        LoginActivityCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var actor = principal.ToActor();

        var response = await handler.ExecuteAsync(
            actor,
            request,
            cancellationToken);

        return Results.Ok(response);
    }

    public static async Task<IResult> TrackActivity(
       TrackActivityRequest request,
       ClaimsPrincipal user,
       IActivityRepository activityRepository,
       TrackActivityCommandHandler handler,
       CancellationToken cancellationToken)
    {
        int? userId = null;

        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        await handler.ExecuteAsync(request, userId, cancellationToken);

        return Results.NoContent();
    }

}