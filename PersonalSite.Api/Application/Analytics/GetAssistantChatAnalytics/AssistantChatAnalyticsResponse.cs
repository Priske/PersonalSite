namespace PersonalSite.Api.Application.Analytics
    .GetAssistantChatAnalytics;

public sealed record AssistantChatAnalyticsResponse(
    AssistantChatAnalyticsSummary Summary,
    IReadOnlyCollection<AssistantChatActivityResponse> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
