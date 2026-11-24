namespace PersonalSite.Api.Application.Analytics.GetContactLinkActivity;

public sealed record ContactLinkAnalyticsResponse(
    int TotalClicks,
    IReadOnlyCollection<ContactLinkAnalyticsItem> Links)
{
    public IReadOnlyList<DailyCount> Daily { get; init; } = [];
    public IReadOnlyList<ContactLinkDailySeries> DailyByLink { get; init; } = [];
}

public sealed record ContactLinkDailySeries(string Label, IReadOnlyList<DailyCount> Daily);
