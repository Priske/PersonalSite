namespace PersonalSite.Api.Application.Analytics.GetDeleteUserActivity;

public sealed record DeleteUserAnalyticsResponse(
    DeleteUserAnalyticsSummary Summary,
    IReadOnlyCollection<DeleteUserActivityResponse> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
