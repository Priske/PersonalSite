namespace PersonalSite.Api.Application.Analytics.GetVideoActivity;

public sealed record GetVideoAnalyticsRequest(
    string? Search,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? SortBy,
    bool? Descending);
