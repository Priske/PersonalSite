namespace PersonalSite.Api.Application.Analytics
    .GetAssistantChatAnalytics;

public sealed record AssistantChatAnalyticsSummary(
    int TotalChats,
    int AuthenticatedChats,
    int AnonymousChats);
