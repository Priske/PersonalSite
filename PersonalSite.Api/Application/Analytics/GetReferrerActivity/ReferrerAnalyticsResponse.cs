namespace PersonalSite.Api.Application.Analytics.GetReferrerActivity;

public sealed record ReferrerAnalyticsResponse(
    int TotalPageViews,
    IReadOnlyCollection<ReferrerAnalyticsItem> Referrers);
