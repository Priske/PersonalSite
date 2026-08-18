namespace PersonalSite.Api.Application.Analytics.GetDeleteUserActivity;

public sealed record GetDeleteUserAnalyticsRequest(
    int? UserId,
    string? Search,
    bool? Successful,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? SortBy,
    bool? Descending,
    int? Page,
    int? PageSize);
