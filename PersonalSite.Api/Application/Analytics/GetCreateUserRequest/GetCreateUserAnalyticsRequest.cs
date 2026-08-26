namespace PersonalSite.Api.Application.Analytics.GetCreateUserRequest;

public sealed record GetCreateUserAnalyticsRequest(
    string? Search,
    bool? Successful,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? SortBy,
    bool? Descending,
    int? Page,
    int? PageSize);
