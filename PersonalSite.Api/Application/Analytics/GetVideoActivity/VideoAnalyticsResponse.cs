namespace PersonalSite.Api.Application.Analytics.GetVideoActivity;

public sealed record VideoAnalyticsResponse(
    int TotalPlays,
    int TotalCompletions,
    decimal TotalWatchedSeconds,
    IReadOnlyCollection<VideoAnalyticsItem> Videos);
