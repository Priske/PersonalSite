namespace PersonalSite.Api.Application.Analytics
    .GetAssistantChatAnalytics;

public sealed record GetAssistantChatAnalyticsRequest(
    int? UserId,
    string? Search,
    DateTimeOffset? From,
    DateTimeOffset? To,
    string? SortBy,
    bool? Descending,
    int? Page,
    int? PageSize);
