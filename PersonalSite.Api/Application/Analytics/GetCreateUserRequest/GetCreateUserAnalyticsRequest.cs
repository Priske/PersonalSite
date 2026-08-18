namespace PersonalSite.Api.Application.Analytics.GetCreateUserRequest.cs;

public sealed record GetCreateUserAnalyticsRequest(
    string? Search,
    bool? Successful,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? SortBy,
    bool? Descending,
    int? Page,
    int? PageSize);


public sealed record CreateUserAnalyticsResponse(
    CreateUserAnalyticsSummary Summary,
    IReadOnlyCollection<CreateUserActivityResponse> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);


public sealed record CreateUserAnalyticsSummary(
    int TotalCreatedUsers);

public sealed record CreateUserActivityResponse(
    int Id,
    int? UserId,
    string? Name,
    string? Email,
    DateTimeOffset CreatedAt);
