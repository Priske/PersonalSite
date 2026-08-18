namespace PersonalSite.Api.Application.Analytics.GetDeleteUserActivity;

public sealed record DeleteUserAnalyticsSummary(
    int TotalAttempts,
    int SuccessfulDeletes,
    int FailedDeletes);
