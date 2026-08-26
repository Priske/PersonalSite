using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Analytics
    .GetAssistantChatAnalytics;

public sealed class AssistantChatActivityCommandHandler(
    IActivityRepository activityRepository) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<AssistantChatAnalyticsResponse> ExecuteAsync(
        Actor actor,
        GetAssistantChatAnalyticsRequest request,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new UnauthorizedAccessException();
        }

        var page = Math.Max(
            MinPage,
            request.Page ?? DefaultPage);

        var pageSize = Math.Clamp(
            request.PageSize ?? DefaultPageSize,
            MinPage,
            MaxPageSize);

        var activities = (
            await activityRepository.GetAsync(
                ActivityType.AssistantChatLog,
                request.From,
                request.To,
                cancellationToken))
            .ToList();

        if (request.UserId is not null)
        {
            activities = activities
                .Where(activity =>
                    activity.UserId == request.UserId)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(
                request.Search))
        {
            var search = request.Search.Trim();

            activities = activities
                .Where(activity =>
                    GetMetadataValue(
                        activity,
                        "question")
                    ?.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) == true ||
                    GetMetadataValue(
                        activity,
                        "answer")
                    ?.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }

        var totalItems = activities.Count;

        var authenticatedChats =
            activities.Count(activity =>
                activity.UserId is not null);

        var anonymousChats =
            activities.Count(activity =>
                activity.UserId is null);

        var descending =
            request.Descending ?? true;

        var sortedActivities =
            SortActivities(
                activities,
                request.SortBy,
                descending);

        var items = sortedActivities
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(activity =>
                new AssistantChatActivityResponse(
                    activity.Id,
                    activity.UserId,
                    GetMetadataValue(
                        activity,
                        "question") ?? string.Empty,
                    GetMetadataValue(
                        activity,
                        "answer") ?? string.Empty,
                    activity.CreatedAt))
            .ToList();

        var totalPages =
            (int)Math.Ceiling(
                totalItems / (double)pageSize);

        return new AssistantChatAnalyticsResponse(
            new AssistantChatAnalyticsSummary(
                totalItems,
                authenticatedChats,
                anonymousChats),
            items,
            page,
            pageSize,
            totalItems,
            totalPages);
    }

    private static IEnumerable<Activity> SortActivities(
        IEnumerable<Activity> activities,
        string? sortBy,
        bool descending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "userid" when descending =>
                activities.OrderByDescending(
                    activity => activity.UserId),

            "userid" =>
                activities.OrderBy(
                    activity => activity.UserId),

            _ when descending =>
                activities.OrderByDescending(
                    activity => activity.CreatedAt),

            _ =>
                activities.OrderBy(
                    activity => activity.CreatedAt)
        };
    }

    private static string? GetMetadataValue(
        Activity activity,
        string key)
    {
        foreach (var metadata in activity.Metadata)
        {
            if (
                metadata.Values.TryGetValue(
                    key,
                    out var value) &&
                value is StringMetadataValue
                    stringValue)
            {
                return stringValue.Value;
            }
        }

        return null;
    }
}