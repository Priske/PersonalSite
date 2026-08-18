namespace PersonalSite.Api.Application.Analytics.GetLoginActivity;

public sealed record LoginAnalyticsResponse(
    LoginAnalyticsSummary Summary,
    IReadOnlyCollection<LoginActivityResponse> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
