namespace PersonalSite.Api.Application.Analytics.GetReferrerActivity;

public sealed record GetReferrerAnalyticsRequest(
    string? Search,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? SortBy,
    bool? Descending
);
