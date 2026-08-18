namespace PersonalSite.Api.Application.Analytics.GetReferrerActivity;

public sealed record ReferrerAnalyticsItem(
    string Referrer,
    int Count);