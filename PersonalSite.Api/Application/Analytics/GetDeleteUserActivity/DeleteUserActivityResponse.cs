namespace PersonalSite.Api.Application.Analytics.GetDeleteUserActivity;

public sealed record DeleteUserActivityResponse(
    int Id,
    int? UserId,
    int? TargetUserId,
    DateTimeOffset CreatedAt,
    bool Successful,
    string? FailureReason);