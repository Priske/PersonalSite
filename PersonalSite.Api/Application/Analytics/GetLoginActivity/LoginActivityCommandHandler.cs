using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Analytics.GetLoginActivity;

public sealed class LoginActivityCommandHandler(
    IActivityRepository activityRepository) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<LoginAnalyticsResponse> ExecuteAsync(
        Actor actor,
        GetLoginAnalyticsRequest request,
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

        var successfulActivities = await activityRepository.GetAsync(
            ActivityType.Login,
            request.From,
            request.To,
            cancellationToken);

        var failedActivities = await activityRepository.GetAsync(
            ActivityType.LoginFailed,
            request.From,
            request.To,
            cancellationToken);

        var allActivities = successfulActivities
            .Concat(failedActivities)
            .ToList();

        if (request.UserId is not null)
        {
            allActivities = allActivities
                .Where(activity =>
                    activity.UserId == request.UserId)
                .ToList();
        }

        if (request.Successful is true)
        {
            allActivities = allActivities
                .Where(activity =>
                    activity.Type == ActivityType.Login)
                .ToList();
        }

        if (request.Successful is false)
        {
            allActivities = allActivities
                .Where(activity =>
                    activity.Type == ActivityType.LoginFailed)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            allActivities = allActivities
                .Where(activity =>
                    activity.UserId?.ToString()
                        .Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) == true ||
                    GetFailureReason(activity)?.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }

        var totalItems = allActivities.Count;

        var successfulLogins = allActivities.Count(activity =>
            activity.Type == ActivityType.Login);

        var failedLogins = allActivities.Count(activity =>
            activity.Type == ActivityType.LoginFailed);

        var unknownEmailAttempts = allActivities.Count(activity =>
            GetFailureReason(activity) == "unknown_email");

        var incorrectPasswordAttempts = allActivities.Count(activity =>
            GetFailureReason(activity) == "incorrect_password");

        var activities = allActivities
            .OrderByDescending(activity => activity.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(activity => new LoginActivityResponse(
                activity.Id,
                activity.UserId,
                activity.CreatedAt,
                activity.Type == ActivityType.Login,
                GetFailureReason(activity)))
            .ToList();

        var totalPages =
            (int)Math.Ceiling(
                totalItems / (double)pageSize);

        return new LoginAnalyticsResponse(
            new LoginAnalyticsSummary(
                totalItems,
                successfulLogins,
                failedLogins,
                unknownEmailAttempts,
                incorrectPasswordAttempts),
            activities,
            page,
            pageSize,
            totalItems,
            totalPages);
    }

    private static string? GetFailureReason(Activity activity)
    {
        foreach (var metadata in activity.Metadata)
        {
            if (metadata.Values.TryGetValue(
                    "reason",
                    out var value) &&
                value is StringMetadataValue stringValue)
            {
                return stringValue.Value;
            }
        }

        return null;
    }
}