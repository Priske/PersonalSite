
using System.Security.Claims;
using System.Text.Json;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
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
       CancellationToken cancellationToken)
    {
        int? userId = null;

        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        var activity = new Activity(
            request.Type,
            userId);

        foreach (var metadataRequest in request.Metadata)
        {
            var metadata = new ActivityMetadata();

            metadata.Add(
                metadataRequest.Key,
                MapValue(metadataRequest.Value));

            activity.AddMetadata(metadata);
        }

        await activityRepository.AddAsync(
            activity,
            cancellationToken);

        return Results.NoContent();
    }


    private static IMetadataValue MapValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String =>
                new StringMetadataValue(value.GetString()!),

            JsonValueKind.Number when value.TryGetInt32(out var integer) =>
                new IntegerMetadataValue(integer),

            JsonValueKind.Number =>
                new DecimalMetadataValue(value.GetDecimal()),

            JsonValueKind.True =>
                new BooleanMetadataValue(true),

            JsonValueKind.False =>
                new BooleanMetadataValue(false),

            _ => throw new ArgumentException("Unsupported metadata value.")
        };
    }
}