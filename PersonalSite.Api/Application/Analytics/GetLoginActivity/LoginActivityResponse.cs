namespace PersonalSite.Api.Application.Analytics.GetLoginActivity;

public sealed record LoginActivityResponse(
    int Id,
    int? UserId,
    DateTimeOffset CreatedAt,
    bool Successful,
    string? FailureReason);
