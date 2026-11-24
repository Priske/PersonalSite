using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Application.Analytics;
using PersonalSite.Api.Application.Analytics.GetContactLinkActivity;
using PersonalSite.Api.Application.Analytics.GetCreateUserRequest;
using PersonalSite.Api.Application.Analytics.GetDeleteUserActivity;
using PersonalSite.Api.Application.Analytics.GetLoginActivity;
using PersonalSite.Api.Application.Analytics.GetReferrerActivity;
using PersonalSite.Api.Application.Analytics.GetVideoActivity;
using PersonalSite.Api.Application.Redirects;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Endpoints.Analytics;
using PersonalSite.Api.Storage.Analytics;

namespace PersonalSite.Api.Tests.Application.Analytics;

// These tests use an in-memory repository, not a database or test server.
public sealed class AnalyticsVisualisationTests
{
    private static readonly Actor Administrator = new(1, UserRole.Administrator);

    [Theory]
    [InlineData("2026-03-28T23:30:00Z", "Europe/Brussels", "2026-03-29")]
    [InlineData("2026-03-29T22:30:00Z", "Europe/Brussels", "2026-03-30")]
    [InlineData("2026-10-25T00:30:00Z", "Europe/Brussels", "2026-10-25")]
    [InlineData("2026-10-25T01:30:00Z", "Europe/Brussels", "2026-10-25")]
    [InlineData("2026-03-29T00:30:00Z", "America/Los_Angeles", "2026-03-28")]
    [InlineData("2026-03-29T00:30:00Z", null, "2026-03-29")]
    public void Calendar_uses_named_zone_and_daylight_saving_rules(string timestamp, string? zone, string expected)
    {
        var day = Assert.Single(AnalyticsCalendar.Counts([DateTimeOffset.Parse(timestamp)], zone));
        Assert.Equal(DateOnly.Parse(expected), day.Date);
        Assert.Equal(1, day.Count);
    }

    [Fact]
    public void Empty_aggregates_do_not_invent_days_or_counts()
    {
        Assert.Empty(AnalyticsCalendar.Counts([], null));
        Assert.Empty(AnalyticsCalendar.Outcomes([], null));
    }

    [Theory]
    [InlineData("?timeZone=Not%2FAZone")]
    [InlineData("?timeZone=")]
    [InlineData("?from=2026-03-30T00%3A00%3A00Z&to=2026-03-29T00%3A00%3A00Z")]
    public async Task Request_filter_rejects_invalid_zone_and_reversed_dates_before_handler(string query)
    {
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString(query);
        var called = false;
        var result = await new AnalyticsDateFilter().InvokeAsync(new DefaultEndpointFilterInvocationContext(context), _ =>
        {
            called = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        });
        Assert.False(called);
        Assert.Equal(400, Assert.IsAssignableFrom<IStatusCodeHttpResult>(result).StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?timeZone=Europe%2FBrussels")]
    public async Task Request_filter_accepts_legacy_and_named_zone_requests(string query)
    {
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString(query);
        var called = false;
        await new AnalyticsDateFilter().InvokeAsync(new DefaultEndpointFilterInvocationContext(context), _ =>
        {
            called = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        });
        Assert.True(called);
    }

    [Fact]
    public async Task Login_trends_include_every_matching_event_before_pagination()
    {
        var repository = new MemoryActivities(
            Event(ActivityType.Login, "2026-03-29T00:00:00Z"),
            Event(ActivityType.Login, "2026-03-29T02:00:00Z"),
            Event(ActivityType.LoginFailed, "2026-03-29T22:30:00Z"),
            Event(ActivityType.LoginFailed, "2026-03-01T00:00:00Z"));
        var handler = new LoginActivityCommandHandler(repository);
        var request = new GetLoginAnalyticsRequest(null, null, null,
            DateTimeOffset.Parse("2026-03-28T23:00:00Z"), DateTimeOffset.Parse("2026-03-30T21:59:59Z"),
            null, null, 2, 1, "Europe/Brussels");
        var result = await handler.ExecuteAsync(Administrator, request, default);
        Assert.Single(result.Items);
        Assert.Equal(3, result.Summary.TotalAttempts);
        Assert.Equal(3, result.Daily.Sum(day => day.Successful + day.Failed));
        Assert.Equal(new DailyOutcome(new DateOnly(2026, 3, 29), 2, 0), result.Daily[0]);
        Assert.Equal(new DailyOutcome(new DateOnly(2026, 3, 30), 0, 1), result.Daily[1]);
        var failed = await handler.ExecuteAsync(Administrator, request with { Successful = false, Page = 1 }, default);
        Assert.Equal(1, failed.Summary.TotalAttempts);
        Assert.Equal(1, Assert.Single(failed.Daily).Failed);
        Assert.Equal(0, failed.Daily.Sum(day => day.Successful));
    }

    [Fact]
    public async Task Referrer_matching_total_and_trend_follow_search_without_changing_legacy_total()
    {
        var repository = new MemoryActivities(
            Event(ActivityType.PageViewed, "2026-03-29T00:00:00Z", "Page", Text("referrer", "https://github.com/a")),
            Event(ActivityType.PageViewed, "2026-03-29T22:30:00Z", "Page", Text("referrer", "https://github.com/b")),
            Event(ActivityType.PageViewed, "2026-03-29T00:00:00Z", "Page", Text("referrer", "https://example.test")));
        var result = await new ReferrerActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new("github", null, null, "count", true, "Europe/Brussels"), default);
        Assert.Equal(3, result.TotalPageViews);
        Assert.Equal(2, result.MatchingPageViews);
        Assert.Equal(2, Assert.Single(result.Referrers).Count);
        Assert.Equal(2, result.Daily.Sum(day => day.Count));
        Assert.Equal(2, result.Daily.Count);
        var series = Assert.Single(result.DailyByReferrer);
        Assert.Equal("github.com", series.Referrer);
        Assert.Equal(result.Daily, series.Daily);

        var unfiltered = await new ReferrerActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new(null, null, null, "referrer", false, "Europe/Brussels"), default);
        Assert.Equal(2, unfiltered.DailyByReferrer.Count);
        Assert.Equal(unfiltered.MatchingPageViews, unfiltered.DailyByReferrer.Sum(item => item.Daily.Sum(day => day.Count)));
        Assert.Equal(new DateOnly(2026, 3, 30), unfiltered.DailyByReferrer.Single(item => item.Referrer == "github.com").Daily[1].Date);
    }

    [Fact]
    public async Task Contact_trends_exclude_redirects_and_follow_label_search()
    {
        var repository = new MemoryActivities(
            Event(ActivityType.LinkClicked, "2026-03-29T00:00:00Z", "Link", Text("section", "contact"), Text("label", "github")),
            Event(ActivityType.LinkClicked, "2026-03-29T22:30:00Z", "Link", Text("section", "contact"), Text("label", "github")),
            Event(ActivityType.LinkClicked, "2026-03-29T00:00:00Z", "Link", Text("section", "contact"), Text("label", "email")),
            Event(ActivityType.LinkClicked, "2026-03-29T00:00:00Z", "Redirect", Text("slug", "cv")));
        var result = await new ContactLinkActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new("github", null, null, null, null, "Europe/Brussels"), default);
        Assert.Equal(2, result.TotalClicks);
        Assert.Equal(2, result.Daily.Sum(day => day.Count));
        Assert.Equal("github", Assert.Single(result.Links).Label);
        var series = Assert.Single(result.DailyByLink);
        Assert.Equal("github", series.Label);
        Assert.Equal(result.Daily, series.Daily);
    }

    [Fact]
    public async Task Video_trends_keep_counts_and_duration_separate_and_follow_search()
    {
        (string, IMetadataValue)[] Video(string name, decimal seconds = 0) =>
        [Text("fileName", name), ("fileId", new IntegerMetadataValue(name == "matching.mp4" ? 1 : 2)),
            ("featuredContentId", new IntegerMetadataValue(7)), ("watchedSeconds", new DecimalMetadataValue(seconds))];
        var repository = new MemoryActivities(
            Event(ActivityType.VideoStarted, "2026-03-29T00:00:00Z", "Video", Video("matching.mp4")),
            Event(ActivityType.VideoWatched, "2026-03-29T22:30:00Z", "Video", Video("matching.mp4", 12.5m)),
            Event(ActivityType.VideoCompleted, "2026-03-29T22:30:00Z", "Video", Video("matching.mp4")),
            Event(ActivityType.VideoStarted, "2026-03-29T00:00:00Z", "Video", Video("other.mp4")));
        var result = await new VideoActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new("matching", null, null, null, null, "Europe/Brussels"), default);
        Assert.Equal(1, result.TotalPlays);
        Assert.Equal(1, result.TotalCompletions);
        Assert.Equal(12.5m, result.TotalWatchedSeconds);
        Assert.Equal(result.TotalPlays, result.Daily.Sum(day => day.Plays));
        Assert.Equal(result.TotalCompletions, result.Daily.Sum(day => day.Completions));
        Assert.Equal(result.TotalWatchedSeconds, result.Daily.Sum(day => day.WatchedSeconds));
        Assert.Equal(2, result.Daily.Count);
        var series = Assert.Single(result.DailyByVideo);
        Assert.Equal("matching.mp4", series.FileName);
        Assert.Equal(result.Daily, series.Daily);
    }

    [Fact]
    public async Task Registrations_and_deletions_aggregate_filtered_records_before_paging()
    {
        var failed = Event(ActivityType.DeleteUser, "2026-03-29T22:30:00Z");
        var reason = new ActivityMetadata();
        reason.Add("reason", new StringMetadataValue("unknown_delete_user"));
        failed.AddMetadata(reason);
        var repository = new MemoryActivities(
            Event(ActivityType.CreatedUser, "2026-03-29T00:00:00Z", "CreatedUser", Text("Name", "Matching")),
            Event(ActivityType.CreatedUser, "2026-03-29T22:30:00Z", "CreatedUser", Text("Name", "Matching")),
            Event(ActivityType.CreatedUser, "2026-03-29T00:00:00Z", "CreatedUser", Text("Name", "Other")),
            Event(ActivityType.DeleteUser, "2026-03-29T00:00:00Z"), failed);
        var registrations = await new CreateUserActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new("Matching", null, null, null, null, null, 2, 1, "Europe/Brussels"), default);
        Assert.Single(registrations.Items);
        Assert.Equal(2, registrations.Summary.TotalCreatedUsers);
        Assert.Equal(2, registrations.Daily.Sum(day => day.Count));
        var deletions = await new DeleteUserActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new(null, "unknown", false, null, null, null, null, 1, 1, "Europe/Brussels"), default);
        Assert.Equal(1, deletions.Summary.TotalAttempts);
        Assert.Equal(1, Assert.Single(deletions.Daily).Failed);
        Assert.Equal(0, deletions.Daily.Sum(day => day.Successful));
    }

    [Fact]
    public async Task Redirect_trends_retain_dates_and_match_filtered_campaign_totals()
    {
        var repository = new MemoryActivities(
            Event(ActivityType.LinkClicked, "2026-03-29T00:00:00Z", "Redirect", Text("slug", "cv"), Text("campaign", "chosen")),
            Event(ActivityType.LinkClicked, "2026-03-29T22:30:00Z", "Redirect", Text("slug", "cv"), Text("campaign", "chosen")),
            Event(ActivityType.LinkClicked, "2026-03-29T00:00:00Z", "Redirect", Text("slug", "cv"), Text("campaign", "other")),
            Event(ActivityType.LinkClicked, "2026-03-29T00:00:00Z", "Link", Text("label", "contact")));
        // AnalyticsAsync uses only its activity repository, not the other dependencies.
        var handler = new RedirectLinkHandler(null!, null!, repository, NullLogger<RedirectLinkHandler>.Instance);
        var result = await handler.AnalyticsAsync(new(null, null, "chosen", null, null, "Europe/Brussels"), default);
        Assert.Equal(2, result.TotalVisits);
        Assert.Equal(2, Assert.Single(result.Links).Visits);
        Assert.Equal(2, result.Daily.Sum(day => day.Count));
        Assert.Equal(2, result.Daily.Count);
        var series = Assert.Single(result.DailyByLink);
        Assert.Equal("cv", series.Slug);
        Assert.Equal(result.Daily, series.Daily);
    }

    [Fact]
    public async Task Category_trends_keep_four_sources_separate_with_reconciled_daily_totals()
    {
        var events = new List<Activity>();
        for (var index = 0; index < 4; index++)
        {
            foreach (var date in new[] { "2026-03-29T00:00:00Z", "2026-03-29T22:30:00Z" })
            {
                events.Add(Event(ActivityType.PageViewed, date, "Page", Text("referrer", $"https://source-{index}.test/")));
                events.Add(Event(ActivityType.LinkClicked, date, "Link", Text("section", "contact"), Text("label", $"contact-{index}")));
                events.Add(Event(ActivityType.LinkClicked, date, "Redirect", Text("slug", $"cv-{index}"), Text("campaign", "chosen")));
            }
            (string, IMetadataValue)[] Video(decimal seconds = 0) =>
            [Text("fileName", index < 2 ? "same-name.mp4" : $"video-{index}.mp4"),
                ("fileId", new IntegerMetadataValue(index + 1)), ("featuredContentId", new IntegerMetadataValue(7)),
                ("watchedSeconds", new DecimalMetadataValue(seconds))];
            events.Add(Event(ActivityType.VideoStarted, "2026-03-29T00:00:00Z", "Video", Video()));
            events.Add(Event(ActivityType.VideoCompleted, "2026-03-29T22:30:00Z", "Video", Video()));
            events.Add(Event(ActivityType.VideoWatched, "2026-03-29T22:30:00Z", "Video", Video(index + 0.25m)));
        }
        var repository = new MemoryActivities(events.ToArray());
        var referrers = await new ReferrerActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new(null, null, null, null, null, "Europe/Brussels"), default);
        Assert.Equal(4, referrers.DailyByReferrer.Count);
        Assert.Equal(referrers.MatchingPageViews, referrers.DailyByReferrer.Sum(series => series.Daily.Sum(day => day.Count)));

        var contacts = await new ContactLinkActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new(null, null, null, null, null, "Europe/Brussels"), default);
        Assert.Equal(4, contacts.DailyByLink.Count);
        Assert.Equal(8, contacts.TotalClicks);
        Assert.Equal(contacts.TotalClicks, contacts.DailyByLink.Sum(series => series.Daily.Sum(day => day.Count)));
        Assert.All(contacts.DailyByLink, series => Assert.Equal(2, series.Daily.Count));

        var redirects = await new RedirectLinkHandler(null!, null!, repository, NullLogger<RedirectLinkHandler>.Instance)
            .AnalyticsAsync(new(null, null, null, null, null, "Europe/Brussels"), default);
        Assert.Equal(4, redirects.DailyByLink.Count);
        Assert.Equal(8, redirects.TotalVisits);
        Assert.Equal(redirects.TotalVisits, redirects.DailyByLink.Sum(series => series.Daily.Sum(day => day.Count)));
        Assert.All(redirects.DailyByLink, series => Assert.Equal(2, series.Daily.Count));

        var videos = await new VideoActivityCommandHandler(repository).ExecuteAsync(Administrator,
            new(null, null, null, null, null, "Europe/Brussels"), default);
        Assert.Equal(4, videos.DailyByVideo.Count);
        Assert.Equal(2, videos.DailyByVideo.Count(series => series.FileName == "same-name.mp4"));
        Assert.Equal(4, videos.DailyByVideo.Select(series => series.FileId).Distinct().Count());
        Assert.Equal(4, videos.TotalPlays);
        Assert.Equal(4, videos.TotalCompletions);
        Assert.Equal(7m, videos.TotalWatchedSeconds);
        Assert.Equal(videos.TotalPlays, videos.DailyByVideo.Sum(series => series.Daily.Sum(day => day.Plays)));
        Assert.Equal(videos.TotalCompletions, videos.DailyByVideo.Sum(series => series.Daily.Sum(day => day.Completions)));
        Assert.Equal(videos.TotalWatchedSeconds, videos.DailyByVideo.Sum(series => series.Daily.Sum(day => day.WatchedSeconds)));
        Assert.All(videos.DailyByVideo, series => Assert.Equal(new DateOnly(2026, 3, 30), series.Daily[1].Date));
    }

    [Fact]
    public async Task Existing_handlers_still_reject_regular_users()
    {
        var repository = new MemoryActivities();
        var user = new Actor(2, UserRole.User);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new LoginActivityCommandHandler(repository).ExecuteAsync(user, new(null, null, null, null, null, null, null, null, null), default));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new ReferrerActivityCommandHandler(repository).ExecuteAsync(user, new(null, null, null, null, null), default));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new ContactLinkActivityCommandHandler(repository).ExecuteAsync(user, new(null, null, null, null, null), default));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new VideoActivityCommandHandler(repository).ExecuteAsync(user, new(null, null, null, null, null), default));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new CreateUserActivityCommandHandler(repository).ExecuteAsync(user, new(null, null, null, null, null, null, null, null), default));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new DeleteUserActivityCommandHandler(repository).ExecuteAsync(user, new(null, null, null, null, null, null, null, null, null), default));
    }

    private static (string, IMetadataValue) Text(string key, string value) => (key, new StringMetadataValue(value));

    private static Activity Event(ActivityType type, string date, string? key = null, params (string Key, IMetadataValue Value)[] values)
    {
        var activity = new Activity(type, 1);
        typeof(Activity).GetProperty(nameof(Activity.CreatedAt))!.SetValue(activity, DateTimeOffset.Parse(date));
        if (key is not null)
        {
            var details = new ObjectMetadataValue();
            foreach (var (name, value) in values) details.Add(name, value);
            var metadata = new ActivityMetadata();
            metadata.Add(key, details);
            activity.AddMetadata(metadata);
        }
        return activity;
    }

    private sealed class MemoryActivities(params Activity[] activities) : IActivityRepository
    {
        public Task AddAsync(Activity activity, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<IReadOnlyList<Activity>> GetAsync(ActivityType type, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Activity>>(activities.Where(activity => activity.Type == type &&
                (from is null || activity.CreatedAt >= from) && (to is null || activity.CreatedAt <= to)).ToList());
    }
}
