using PersonalSite.Api.Application.Analytics;

namespace PersonalSite.Api.Application.Redirects;

public sealed record SaveRedirectLinkRequest(string TargetUrl, string? Campaign, bool IsActive);

public sealed record RedirectLinkResponse(
    string Slug, string TargetUrl, string? Campaign, bool IsActive, DateTimeOffset CreatedAt);

public sealed record RedirectAnalyticsRequest(
    DateTimeOffset? From, DateTimeOffset? To, string? Search, string? SortBy, bool? Descending,
    string? TimeZone = null);

public sealed record RedirectAnalyticsItem(string Slug, string? Campaign, string Referrer, int Visits);

public sealed record RedirectAnalyticsResponse(int TotalVisits, IReadOnlyList<RedirectAnalyticsItem> Links)
{
    public IReadOnlyList<DailyCount> Daily { get; init; } = [];
    public IReadOnlyList<RedirectDailySeries> DailyByLink { get; init; } = [];
}

public sealed record RedirectDailySeries(string Slug, IReadOnlyList<DailyCount> Daily);
