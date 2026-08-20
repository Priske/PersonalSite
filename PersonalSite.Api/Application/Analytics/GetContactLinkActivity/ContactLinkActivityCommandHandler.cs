using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Analytics.GetContactLinkActivity;

public sealed class ContactLinkActivityCommandHandler(
    IActivityRepository activityRepository) : IHandler
{
    public async Task<ContactLinkAnalyticsResponse> ExecuteAsync(
        Actor actor,
        GetContactLinkAnalyticsRequest request,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new UnauthorizedAccessException();
        }

        var activities = await activityRepository.GetAsync(
            ActivityType.LinkClicked,
            request.From,
            request.To,
            cancellationToken);

        var links = activities
            .Select(GetContactLabel)
            .Where(label => label is not null)
            .Select(label => label!)
            .GroupBy(label => label)
            .Select(group => new ContactLinkAnalyticsItem(
                group.Key,
                group.Count()))
            .ToList();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            links = links
                .Where(link => link.Label.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var descending = request.Descending ?? true;

        links = request.SortBy?.ToLowerInvariant() switch
        {
            "label" => descending
                ? links.OrderByDescending(link => link.Label).ToList()
                : links.OrderBy(link => link.Label).ToList(),

            _ => descending
                ? links.OrderByDescending(link => link.Clicks).ToList()
                : links.OrderBy(link => link.Clicks).ToList()
        };

        return new ContactLinkAnalyticsResponse(
            links.Sum(link => link.Clicks),
            links);
    }

    private static string? GetContactLabel(Activity activity)
    {
        foreach (var metadata in activity.Metadata)
        {
            if (!metadata.Values.TryGetValue(
                    "Link",
                    out var value) ||
                value is not ObjectMetadataValue link)
            {
                continue;
            }

            if (GetString(link, "section") != "contact")
            {
                continue;
            }

            return GetString(link, "label");
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
}
