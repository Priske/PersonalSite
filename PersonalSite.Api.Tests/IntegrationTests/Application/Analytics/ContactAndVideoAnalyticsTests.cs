using System.Net;
using System.Net.Http.Json;
using PersonalSite.Api.Application.Analytics.GetContactLinkActivity;
using PersonalSite.Api.Application.Analytics.GetVideoActivity;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Analytics;

public sealed class ContactAndVideoAnalyticsTests : IntegrationTest
{
    [Fact]
    public async Task ContactAndVideoActivityCanBeReported()
    {
        await TrackActivity(
            "LinkClicked",
            "Link",
            new
            {
                label = "github",
                destination = "https://github.com/example",
                section = "contact",
                page = "/"
            });

        var video = new
        {
            featuredContentId = 7,
            fileId = 42,
            fileName = "portfolio.mp4",
            positionSeconds = 12.5,
            durationSeconds = 90.0
        };

        await TrackActivity(
            "VideoStarted",
            "Video",
            video);

        await TrackActivity(
            "VideoWatched",
            "Video",
            new
            {
                video.featuredContentId,
                video.fileId,
                video.fileName,
                video.positionSeconds,
                video.durationSeconds,
                watchedSeconds = 12.5,
                reason = "paused"
            });

        await TrackActivity(
            "VideoCompleted",
            "Video",
            video);

        await AuthenticateAsUser(UserRole.Administrator);

        var contactResponse = await Client.GetAsync(
            "/analytics/contact-links");

        var contactAnalytics = await contactResponse
            .ReadJsonAs<ContactLinkAnalyticsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(1, contactAnalytics.TotalClicks);

        var contactLink = Assert.Single(
            contactAnalytics.Links);

        Assert.Equal("github", contactLink.Label);
        Assert.Equal(1, contactLink.Clicks);

        var videoResponse = await Client.GetAsync(
            "/analytics/videos");

        var videoAnalytics = await videoResponse
            .ReadJsonAs<VideoAnalyticsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(1, videoAnalytics.TotalPlays);
        Assert.Equal(1, videoAnalytics.TotalCompletions);
        Assert.Equal(12.5m, videoAnalytics.TotalWatchedSeconds);

        var videoItem = Assert.Single(videoAnalytics.Videos);

        Assert.Equal(7, videoItem.FeaturedContentId);
        Assert.Equal(42, videoItem.FileId);
        Assert.Equal("portfolio.mp4", videoItem.FileName);
        Assert.Equal(1, videoItem.Plays);
        Assert.Equal(1, videoItem.Completions);
        Assert.Equal(12.5m, videoItem.WatchedSeconds);
    }

    private async Task TrackActivity(
        string type,
        string key,
        object value)
    {
        var response = await Client.PostAsJsonAsync(
            "/analytics",
            new
            {
                type,
                metadata = new[]
                {
                    new { key, value }
                }
            });

        await response.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);
    }
}
