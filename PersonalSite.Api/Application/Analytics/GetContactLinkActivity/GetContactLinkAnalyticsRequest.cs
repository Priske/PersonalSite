namespace PersonalSite.Api.Application.Analytics.GetContactLinkActivity;

public sealed record GetContactLinkAnalyticsRequest(
    string? Search,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? SortBy,
    bool? Descending);
