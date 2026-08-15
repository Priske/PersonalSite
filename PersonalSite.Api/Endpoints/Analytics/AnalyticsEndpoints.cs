
using System.Security.Claims;
using PersonalSite.Api.Application.Analytics;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Endpoints.Analytics;

public static class AnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/analytics", TrackActivity);

        return app;
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