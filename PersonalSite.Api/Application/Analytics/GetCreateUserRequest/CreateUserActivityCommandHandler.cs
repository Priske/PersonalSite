using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Analytics.GetCreateUserRequest;

public sealed class CreateUserActivityCommandHandler(
    IActivityRepository activityRepository) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<CreateUserAnalyticsResponse> ExecuteAsync(
        Actor actor,
        GetCreateUserAnalyticsRequest request,
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
            ActivityType.CreatedUser,
            request.From,
            request.To,
            cancellationToken);

        var createdUsers = allActivities
            .Select(activity => new CreateUserActivityResponse(
                activity.Id,
                activity.UserId,
                GetCreatedUserValue(activity, "Name"),
                GetCreatedUserValue(activity, "Email"),
                activity.CreatedAt))
            .ToList();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            createdUsers = createdUsers
                .Where(item =>
                    (item.Name?.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.Email?.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        var descending = request.Descending ?? true;

        createdUsers = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => descending
                ? createdUsers
                    .OrderByDescending(item => item.Name)
                    .ToList()
                : createdUsers
                    .OrderBy(item => item.Name)
                    .ToList(),

            "email" => descending
                ? createdUsers
                    .OrderByDescending(item => item.Email)
                    .ToList()
                : createdUsers
                    .OrderBy(item => item.Email)
                    .ToList(),

            "createdat" => descending
                ? createdUsers
                    .OrderByDescending(item => item.CreatedAt)
                    .ToList()
                : createdUsers
                    .OrderBy(item => item.CreatedAt)
                    .ToList(),

            _ => createdUsers
                .OrderByDescending(item => item.CreatedAt)
                .ToList()
        };

        var totalItems = createdUsers.Count;

        var items = createdUsers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPages =
            (int)Math.Ceiling(
                totalItems / (double)pageSize);

        return new CreateUserAnalyticsResponse(
            new CreateUserAnalyticsSummary(
                totalItems),
            items,
            page,
            pageSize,
            totalItems,
            totalPages);
    }

    private static string? GetCreatedUserValue(
        Activity activity,
        string key)
    {
        foreach (var metadata in activity.Metadata)
        {
            if (!metadata.Values.TryGetValue(
                    "CreatedUser",
                    out var createdUserValue))
            {
                continue;
            }

            if (createdUserValue is not ObjectMetadataValue createdUser)
            {
                continue;
            }

            if (!createdUser.Values.TryGetValue(
                    key,
                    out var value))
            {
                continue;
            }

            if (value is StringMetadataValue stringValue)
            {
                return stringValue.Value;
            }
        }

        return null;
    }
}