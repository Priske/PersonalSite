namespace PersonalSite.Api.Application.Analytics
    .GetAssistantChatAnalytics;

public sealed record AssistantChatActivityResponse(
    int Id,
    int? UserId,
    string Question,
    string Answer,
    DateTimeOffset CreatedAt);