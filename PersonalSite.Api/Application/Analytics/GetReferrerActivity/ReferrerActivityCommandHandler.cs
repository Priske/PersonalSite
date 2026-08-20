using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Analytics.GetReferrerActivity;

public sealed class ReferrerActivityCommandHandler(
    IActivityRepository activityRepository) : IHandler
{
    public async Task<ReferrerAnalyticsResponse> ExecuteAsync(
        Actor actor,
        GetReferrerAnalyticsRequest request,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new UnauthorizedAccessException();
        }

        var allActivities = await activityRepository.GetAsync(
            ActivityType.PageViewed,
            request.From,
            request.To,
            cancellationToken);

        var totalPageViews = allActivities.Count;

        var referrers = allActivities
            .Select(GetReferrer)
            .Select(NormalizeReferrer)
            .GroupBy(referrer => referrer)
            .Select(group => new ReferrerAnalyticsItem(
                group.Key,
                group.Count()))
            .ToList();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            referrers = referrers
                .Where(item =>
                    item.Referrer.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var descending = request.Descending ?? true;

        referrers = request.SortBy?.ToLowerInvariant() switch
        {
            "referrer" => descending
                ? referrers
                    .OrderByDescending(item => item.Referrer)
                    .ToList()
                : referrers
                    .OrderBy(item => item.Referrer)
                    .ToList(),

            "count" => descending
                ? referrers
                    .OrderByDescending(item => item.Count)
                    .ToList()
                : referrers
                    .OrderBy(item => item.Count)
                    .ToList(),

            _ => referrers
                .OrderByDescending(item => item.Count)
                .ToList()
        };

        return new ReferrerAnalyticsResponse(
            totalPageViews,
            referrers);
    }

    private static string? GetReferrer(Activity activity)
    {
        foreach (var metadata in activity.Metadata)
        {
            if (!metadata.Values.TryGetValue(
                    "Page",
                    out var pageValue))
            {
                continue;
            }

            if (pageValue is not ObjectMetadataValue page)
            {
                continue;
            }

            if (!page.Values.TryGetValue(
                    "referrer",
                    out var referrerValue))
            {
                continue;
            }

            if (referrerValue is StringMetadataValue referrer)
            {
                return referrer.Value;
            }
        }

        return null;
    }

    private static string NormalizeReferrer(string? referrer)
    {
        if (string.IsNullOrWhiteSpace(referrer))
        {
            return "Direct";
        }

        if (Uri.TryCreate(
                referrer,
                UriKind.Absolute,
                out var uri))
        {
            return uri.Host;
        }

        return referrer;
    }
}
