using System.Text.Json;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Analytics;

public sealed class TrackActivityCommandHandler(
    IActivityRepository activityRepository) : IHandler
{
    public async Task ExecuteAsync(
        TrackActivityRequest request,
        int? userId,
        CancellationToken cancellationToken)
    {
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

            JsonValueKind.Object =>
                MapObject(value),

            _ => throw new ArgumentException(
                "Unsupported metadata value.")
        };
    }

    private static ObjectMetadataValue MapObject(JsonElement value)
    {
        var metadata = new ObjectMetadataValue();

        foreach (var property in value.EnumerateObject())
        {
            metadata.Add(
                property.Name,
                MapValue(property.Value));
        }

        return metadata;
    }
}