namespace PersonalSite.Api.Application.Analytics.GetLoginActivity;

public sealed record GetLoginAnalyticsRequest(
    int? UserId,
    string? Search,
    bool? Successful,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? SortBy,
    bool? Descending,
    int? Page,
    int? PageSize);
