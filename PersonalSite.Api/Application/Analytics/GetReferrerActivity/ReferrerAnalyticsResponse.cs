namespace PersonalSite.Api.Application.Analytics.GetReferrerActivity;

public sealed record ReferrerAnalyticsResponse(
    int TotalPageViews,
    IReadOnlyCollection<ReferrerAnalyticsItem> Referrers)
{
    public int MatchingPageViews { get; init; }
    public IReadOnlyList<DailyCount> Daily { get; init; } = [];
    public IReadOnlyList<ReferrerDailySeries> DailyByReferrer { get; init; } = [];
}

public sealed record ReferrerDailySeries(string Referrer, IReadOnlyList<DailyCount> Daily);
