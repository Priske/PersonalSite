namespace PersonalSite.Api.Application.Analytics.GetCreateUserRequest;

public sealed record CreateUserAnalyticsResponse(
    CreateUserAnalyticsSummary Summary,
    IReadOnlyCollection<CreateUserActivityResponse> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
