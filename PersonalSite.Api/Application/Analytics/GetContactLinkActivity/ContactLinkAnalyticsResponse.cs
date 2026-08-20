namespace PersonalSite.Api.Application.Analytics.GetContactLinkActivity;

public sealed record ContactLinkAnalyticsResponse(
    int TotalClicks,
    IReadOnlyCollection<ContactLinkAnalyticsItem> Links);
