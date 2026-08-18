namespace PersonalSite.Api.Application.Analytics.GetLoginActivity;

public sealed record LoginAnalyticsSummary(
int TotalAttempts,
int SuccessfulLogins,
int FailedLogins,
int UnknownEmailAttempts,
int IncorrectPasswordAttempts);