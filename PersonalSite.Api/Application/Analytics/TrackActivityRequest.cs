namespace PersonalSite.Api.Application.Analytics;

using PersonalSite.Api.Analytics;

public sealed record TrackActivityRequest(
    ActivityType Type,
    IReadOnlyCollection<ActivityMetadataRequest> Metadata);