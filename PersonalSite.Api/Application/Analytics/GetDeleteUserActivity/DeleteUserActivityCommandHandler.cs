using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Analytics.GetDeleteUserActivity;

public sealed class DeleteUserActivityCommandHandler(
    IActivityRepository activityRepository) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<DeleteUserAnalyticsResponse> ExecuteAsync(
        Actor actor,
        GetDeleteUserAnalyticsRequest request,
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

        var allActivities = await activityRepository.GetAsync(
            ActivityType.DeleteUser,
            request.From,
            request.To,
            cancellationToken);

        var deleteActivities = allActivities
            .Select(activity =>
            {
                var failureReason =
                    GetStringMetadataValue(
                        activity,
                        "reason");

                var targetUserId =
                    GetIntegerMetadataValue(
                        activity,
                        "deleted_user")
                    ??
                    GetIntegerMetadataValue(
                        activity,
                        "attempted_delete_user");

                return new DeleteUserActivityResponse(
                    activity.Id,
                    activity.UserId,
                    targetUserId,
                    activity.CreatedAt,
                    failureReason is null,
                    failureReason);
            })
            .ToList();

        if (request.UserId is not null)
        {
            deleteActivities = deleteActivities
                .Where(activity =>
                    activity.UserId == request.UserId)
                .ToList();
        }

        if (request.Successful is true)
        {
            deleteActivities = deleteActivities
                .Where(activity =>
                    activity.Successful)
                .ToList();
        }

        if (request.Successful is false)
        {
            deleteActivities = deleteActivities
                .Where(activity =>
                    !activity.Successful)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            deleteActivities = deleteActivities
                .Where(activity =>
                    activity.UserId?.ToString()
                        .Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) == true ||
                    activity.TargetUserId?.ToString()
                        .Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) == true ||
                    activity.FailureReason?.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }

        var descending =
            request.Descending ?? true;

        deleteActivities =
            request.SortBy?.ToLowerInvariant() switch
            {
                "userid" => descending
                    ? deleteActivities
                        .OrderByDescending(activity =>
                            activity.UserId)
                        .ToList()
                    : deleteActivities
                        .OrderBy(activity =>
                            activity.UserId)
                        .ToList(),

                "targetuserid" => descending
                    ? deleteActivities
                        .OrderByDescending(activity =>
                            activity.TargetUserId)
                        .ToList()
                    : deleteActivities
                        .OrderBy(activity =>
                            activity.TargetUserId)
                        .ToList(),

                "createdat" => descending
                    ? deleteActivities
                        .OrderByDescending(activity =>
                            activity.CreatedAt)
                        .ToList()
                    : deleteActivities
                        .OrderBy(activity =>
                            activity.CreatedAt)
                        .ToList(),

                _ => deleteActivities
                    .OrderByDescending(activity =>
                        activity.CreatedAt)
                    .ToList()
            };

        var totalItems =
            deleteActivities.Count;

        var successfulDeletes =
            deleteActivities.Count(activity =>
                activity.Successful);

        var failedDeletes =
            deleteActivities.Count(activity =>
                !activity.Successful);

        var items = deleteActivities
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPages =
            (int)Math.Ceiling(
                totalItems / (double)pageSize);

        return new DeleteUserAnalyticsResponse(
            new DeleteUserAnalyticsSummary(
                totalItems,
                successfulDeletes,
                failedDeletes),
            items,
            page,
            pageSize,
            totalItems,
            totalPages);
    }

    private static string? GetStringMetadataValue(
        Activity activity,
        string key)
    {
        foreach (var metadata in activity.Metadata)
        {
            if (metadata.Values.TryGetValue(
                    key,
                    out var value) &&
                value is StringMetadataValue stringValue)
            {
                return stringValue.Value;
            }
        }

        return null;
    }

    private static int? GetIntegerMetadataValue(
        Activity activity,
        string key)
    {
        foreach (var metadata in activity.Metadata)
        {
            if (metadata.Values.TryGetValue(
                    key,
                    out var value) &&
                value is IntegerMetadataValue integerValue)
            {
                return integerValue.Value;
            }
        }

        return null;
    }
}