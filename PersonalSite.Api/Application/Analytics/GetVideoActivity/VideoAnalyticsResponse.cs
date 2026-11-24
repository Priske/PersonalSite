namespace PersonalSite.Api.Application.Analytics.GetVideoActivity;

public sealed record VideoAnalyticsResponse(
    int TotalPlays,
    int TotalCompletions,
    decimal TotalWatchedSeconds,
    IReadOnlyCollection<VideoAnalyticsItem> Videos)
{
    public IReadOnlyList<DailyVideo> Daily { get; init; } = [];
    public IReadOnlyList<VideoDailySeries> DailyByVideo { get; init; } = [];
}

public sealed record VideoDailySeries(int FeaturedContentId, int FileId, string FileName, IReadOnlyList<DailyVideo> Daily);
