namespace PersonalSite.Api.Application.Analytics.GetCreateUserRequest;

public sealed record CreateUserActivityResponse(
    int Id,
    int? UserId,
    string? Name,
    string? Email,
    DateTimeOffset CreatedAt);
