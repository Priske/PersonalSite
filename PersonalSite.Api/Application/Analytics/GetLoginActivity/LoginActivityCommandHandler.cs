using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Analytics.GetLoginActivity;

public sealed class LoginActivityCommandHandler(AppDbContext dbContext) : IHandler
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

        var page = Math.Max(MinPage, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(request.PageSize ?? DefaultPageSize, MinPage, MaxPageSize);

        var query = dbContext.Activities
            .AsNoTracking()
            .Where(activity =>
                activity.Type == ActivityType.Login ||
                activity.Type == ActivityType.LoginFailed);

        if (request.UserId is not null)
        {
            query = query.Where(activity => activity.UserId == request.UserId);
        }

        if (request.Successful is true)
        {
            query = query.Where(activity => activity.Type == ActivityType.Login);
        }

        if (request.Successful is false)
        {
            query = query.Where(activity => activity.Type == ActivityType.LoginFailed);
        }

        if (request.From is not null)
        {
            query = query.Where(activity => activity.CreatedAt >= request.From);
        }

        if (request.To is not null)
        {
            query = query.Where(activity => activity.CreatedAt <= request.To);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var successfulLogins = await query
            .CountAsync(activity => activity.Type == ActivityType.Login, cancellationToken);

        var failedLogins = await query
            .CountAsync(activity => activity.Type == ActivityType.LoginFailed, cancellationToken);

        var allActivities = await query
            .Include(activity => activity.Metadata)
            .ToListAsync(cancellationToken);

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

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

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
            if (metadata.Values.TryGetValue("reason", out var value) &&
                value is StringMetadataValue stringValue)
            {
                return stringValue.Value;
            }
        }

        return null;
    }
}