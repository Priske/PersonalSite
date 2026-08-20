namespace PersonalSite.Api.Application.Analytics.GetVideoActivity;

public sealed record VideoAnalyticsItem(
    int FeaturedContentId,
    int FileId,
    string FileName,
    int Plays,
    int Completions,
    decimal WatchedSeconds);
