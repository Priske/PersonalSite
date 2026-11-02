using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Redirects;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Storage;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Application.Redirects;

public sealed class RedirectLinkHandler(
    AppDbContext dbContext,
    ActivityTracker tracker,
    IActivityRepository activityRepository,
    ILogger<RedirectLinkHandler> logger) : IHandler
{
    public static bool IsSlug(string? slug) => RedirectSlug.IsValid(slug);

    public static Dictionary<string, string[]> Validate(string slug, SaveRedirectLinkRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        try { _ = new RedirectSlug(slug); }
        catch (DomainException exception) { errors["slug"] = [exception.Message]; }
        try { _ = new RedirectTargetUrl(request.TargetUrl); }
        catch (DomainException exception) { errors["targetUrl"] = [exception.Message]; }
        try { _ = RedirectCampaign.CreateOptional(request.Campaign); }
        catch (DomainException exception) { errors["campaign"] = [exception.Message]; }
        return errors;
    }

    public async Task<IReadOnlyList<RedirectLinkResponse>> ListAsync(CancellationToken cancellationToken) =>
        (await dbContext.RedirectLinks.AsNoTracking().OrderBy(link => link.Slug)
            .ToListAsync(cancellationToken)).Select(Response).ToArray();

    public async Task<RedirectLinkResponse> SaveAsync(string slug, SaveRedirectLinkRequest request, CancellationToken cancellationToken)
    {
        // Domain construction also protects callers that bypass endpoint validation.
        var key = new RedirectSlug(slug);
        var target = new RedirectTargetUrl(request.TargetUrl);
        var campaign = RedirectCampaign.CreateOptional(request.Campaign);
        // Atomic upsert prevents concurrent first saves from creating duplicates; later saves win.
        await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO "RedirectLinks" ("Slug", "TargetUrl", "Campaign", "IsActive", "CreatedAt")
            VALUES ({key.Value}, {target.Value}, {campaign?.Value}, {request.IsActive}, {DateTimeOffset.UtcNow})
            ON CONFLICT ("Slug") DO UPDATE SET
                "TargetUrl" = EXCLUDED."TargetUrl", "Campaign" = EXCLUDED."Campaign", "IsActive" = EXCLUDED."IsActive"
            """, cancellationToken);
        var link = await dbContext.RedirectLinks.AsNoTracking().SingleAsync(link => link.Slug == key, cancellationToken);
        return Response(link);
    }

    public async Task<string?> ResolveAsync(string slug, string? referrer, CancellationToken cancellationToken)
    {
        if (!IsSlug(slug)) return null;
        var key = new RedirectSlug(slug);
        var link = await dbContext.RedirectLinks.AsNoTracking()
            .SingleOrDefaultAsync(link => link.Slug == key && link.IsActive, cancellationToken);
        if (link is null) return null;

        try
        {
            // Existing metadata storage performs several saves: commit one complete event or none.
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await tracker.TrackAsync(ActivityType.LinkClicked, null, metadata =>
            {
                var redirect = new ObjectMetadataValue();
                redirect.Add("slug", new StringMetadataValue(link.Slug.Value));
                redirect.Add("campaign", new StringMetadataValue(link.Campaign?.Value ?? string.Empty));
                redirect.Add("referrer", new StringMetadataValue(SanitizeReferrer(referrer)));
                metadata.Add("Redirect", redirect);
            }, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // A telemetry failure must not break the printed link. Do not log URL/header contents.
            logger.LogWarning("Redirect visit could not be recorded; continuing to the saved destination.");
        }

        return link.TargetUrl.Value;
    }

    private static string SanitizeReferrer(string? referrer)
    {
        if (referrer is null || referrer.Length > 2048 || referrer.Any(char.IsControl) ||
            !Uri.TryCreate(referrer, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
            return "Direct / unavailable";
        // Origin only: never persist credentials, potentially private paths, query strings or fragments.
        return new UriBuilder(uri.Scheme, uri.Host, uri.IsDefaultPort ? -1 : uri.Port)
            .Uri.GetLeftPart(UriPartial.Authority);
    }

    public async Task<RedirectAnalyticsResponse> AnalyticsAsync(RedirectAnalyticsRequest request, CancellationToken cancellationToken)
    {
        var activities = await activityRepository.GetAsync(ActivityType.LinkClicked, request.From, request.To, cancellationToken);
        var visits = activities.SelectMany(activity => activity.Metadata)
            .Where(metadata => metadata.Values.TryGetValue("Redirect", out var value) && value is ObjectMetadataValue)
            .Select(metadata => (ObjectMetadataValue)metadata.Values["Redirect"])
            .Select(metadata => new
            {
                Slug = Text(metadata, "slug"),
                Campaign = Text(metadata, "campaign"),
                Referrer = Text(metadata, "referrer")
            })
            .Where(visit => !string.IsNullOrEmpty(visit.Slug))
            .GroupBy(visit => new { visit.Slug, visit.Campaign, visit.Referrer })
            .Select(group => new RedirectAnalyticsItem(group.Key.Slug,
                string.IsNullOrEmpty(group.Key.Campaign) ? null : group.Key.Campaign,
                group.Key.Referrer, group.Count()));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            visits = visits.Where(item => item.Slug.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (item.Campaign?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                item.Referrer.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var descending = request.Descending ?? true;
        var sorted = request.SortBy?.ToLowerInvariant() == "slug"
            ? (descending ? visits.OrderByDescending(item => item.Slug) : visits.OrderBy(item => item.Slug))
            : (descending ? visits.OrderByDescending(item => item.Visits) : visits.OrderBy(item => item.Visits));
        var links = sorted.ThenBy(item => item.Slug).ThenBy(item => item.Campaign).ThenBy(item => item.Referrer).ToList();
        return new(links.Sum(item => item.Visits), links);
    }

    private static string Text(ObjectMetadataValue metadata, string key) =>
        metadata.Values.TryGetValue(key, out var value) && value is StringMetadataValue text ? text.Value : string.Empty;

    private static RedirectLinkResponse Response(RedirectLink link) =>
        new(link.Slug.Value, link.TargetUrl.Value, link.Campaign?.Value, link.IsActive, link.CreatedAt);
}
