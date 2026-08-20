using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Analytics.GetVideoActivity;

public sealed class VideoActivityCommandHandler(
    IActivityRepository activityRepository) : IHandler
{
    public async Task<VideoAnalyticsResponse> ExecuteAsync(
        Actor actor,
        GetVideoAnalyticsRequest request,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new UnauthorizedAccessException();
        }

        var started = await activityRepository.GetAsync(
            ActivityType.VideoStarted,
            request.From,
            request.To,
            cancellationToken);

        var watched = await activityRepository.GetAsync(
            ActivityType.VideoWatched,
            request.From,
            request.To,
            cancellationToken);

        var completed = await activityRepository.GetAsync(
            ActivityType.VideoCompleted,
            request.From,
            request.To,
            cancellationToken);

        var videoActivities = started
            .Concat(watched)
            .Concat(completed)
            .Select(GetVideoActivity)
            .Where(item => item is not null)
            .Select(item => item!)
            .ToList();

        var videos = videoActivities
            .GroupBy(item => new
            {
                item.FeaturedContentId,
                item.FileId,
                item.FileName
            })
            .Select(group => new VideoAnalyticsItem(
                group.Key.FeaturedContentId,
                group.Key.FileId,
                group.Key.FileName,
                group.Count(item =>
                    item.Type == ActivityType.VideoStarted),
                group.Count(item =>
                    item.Type == ActivityType.VideoCompleted),
                group.Sum(item => item.WatchedSeconds)))
            .ToList();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            videos = videos
                .Where(video =>
                    video.FileName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) ||
                    video.FileId.ToString().Contains(search))
                .ToList();
        }

        var descending = request.Descending ?? true;

        videos = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => descending
                ? videos.OrderByDescending(video => video.FileName).ToList()
                : videos.OrderBy(video => video.FileName).ToList(),

            "plays" => descending
                ? videos.OrderByDescending(video => video.Plays).ToList()
                : videos.OrderBy(video => video.Plays).ToList(),

            "completions" => descending
                ? videos.OrderByDescending(video => video.Completions).ToList()
                : videos.OrderBy(video => video.Completions).ToList(),

            _ => descending
                ? videos.OrderByDescending(video => video.WatchedSeconds).ToList()
                : videos.OrderBy(video => video.WatchedSeconds).ToList()
        };

        return new VideoAnalyticsResponse(
            videos.Sum(video => video.Plays),
            videos.Sum(video => video.Completions),
            videos.Sum(video => video.WatchedSeconds),
            videos);
    }

    private static VideoActivityData? GetVideoActivity(
        Activity activity)
    {
        foreach (var metadata in activity.Metadata)
        {
            if (!metadata.Values.TryGetValue(
                    "Video",
                    out var value) ||
                value is not ObjectMetadataValue video)
            {
                continue;
            }

            var featuredContentId = GetInteger(
                video,
                "featuredContentId");

            var fileId = GetInteger(video, "fileId");
            var fileName = GetString(video, "fileName");

            if (featuredContentId is null ||
                fileId is null ||
                fileName is null)
            {
                return null;
            }

            return new VideoActivityData(
                featuredContentId.Value,
                fileId.Value,
                fileName,
                activity.Type,
                GetDecimal(video, "watchedSeconds") ?? 0);
        }

        return null;
    }

    private static string? GetString(
        ObjectMetadataValue metadata,
        string key)
    {
        return metadata.Values.TryGetValue(key, out var value) &&
            value is StringMetadataValue text
                ? text.Value
                : null;
    }

    private static int? GetInteger(
        ObjectMetadataValue metadata,
        string key)
    {
        return metadata.Values.TryGetValue(key, out var value) &&
            value is IntegerMetadataValue integer
                ? integer.Value
                : null;
    }

    private static decimal? GetDecimal(
        ObjectMetadataValue metadata,
        string key)
    {
        if (!metadata.Values.TryGetValue(key, out var value))
        {
            return null;
        }

        return value switch
        {
            DecimalMetadataValue number => number.Value,
            IntegerMetadataValue integer => integer.Value,
            _ => null
        };
    }

    private sealed record VideoActivityData(
        int FeaturedContentId,
        int FileId,
        string FileName,
        ActivityType Type,
        decimal WatchedSeconds);
}
