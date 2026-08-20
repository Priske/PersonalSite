using System.Net;

using PersonalSite.Api.Application.Tags.GetTagDetails;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.FeaturedContent;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Tags.GetTagDetails;

public sealed class GetTagDetailsTests : IntegrationTest
{
    [Fact]
    public async Task GetTagDetailsReturnsTag()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        var tag =
            Tag.Create(
                actor,
                new TagName("C#"));

        Writer.Seed(
            db => db.Tags.Add(tag));

        var response =
            await Client.GetAsync(
                $"/tags/{tag.Id}");

        var result =
            await response.ReadJsonAs<GetTagDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            tag.Id,
            result.Id);

        Assert.Equal(
            "C#",
            result.Name);

        Assert.Empty(
            result.Projects);

        Assert.Equal(
            "Official",
            result.Source);

        Assert.Equal(
            userId,
            result.CreatedByUserId);

        Assert.Equal(
            userId,
            result.LastEditedByUserId);
    }

    [Fact]
    public async Task GetTagDetailsReturnsProjectsUsingTag()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        var tag =
            Tag.Create(
                actor,
                new TagName("C#"));

        var firstProject =
            Project.Create(
                actor,
                new ProjectTitle("Book Tracker"),
                new ProjectDescription(
                    "Tracks books"),
                1,
                new Url(
                    "https://github.com/example/books"),
                null,
                false,
                [tag]);

        var secondProject =
            Project.Create(
                actor,
                new ProjectTitle("Personal Site"),
                new ProjectDescription(
                    "Portfolio"),
                2,
                new Url(
                    "https://github.com/example/site"),
                null,
                false,
                [tag]);

        Writer.Seed(
            db =>
            {
                db.Tags.Add(tag);

                db.Projects.AddRange(
                    firstProject,
                    secondProject);
            });

        var response =
            await Client.GetAsync(
                $"/tags/{tag.Id}");

        var result =
            await response.ReadJsonAs<GetTagDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            2,
            result.Projects.Count);

        Assert.Equal(
            "Book Tracker",
            result.Projects[0].Title);

        Assert.Equal(
            "Personal Site",
            result.Projects[1].Title);
    }

    [Fact]
    public async Task GetTagDetailsReportsFeaturedContentUsage()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        var tag =
            Tag.Create(
                actor,
                new TagName("ASP.NET Core"));

        var featuredContent =
            FeaturedContent.Create(
                actor,
                new FeaturedContentTitle(
                    "Personal Site Walkthrough"),
                new FeaturedContentDescription(
                    "A walkthrough of the personal site."),
                [tag]);

        Writer.Seed(
            db =>
            {
                db.Tags.Add(tag);
                db.FeaturedContents.Add(featuredContent);
            });

        var response =
            await Client.GetAsync(
                $"/tags/{tag.Id}");

        var result =
            await response.ReadJsonAs<GetTagDetailsResponse>(
                HttpStatusCode.OK);

        Assert.True(
            result.IsUsedByFeaturedContent);
    }

    [Fact]
    public async Task GetUnknownTagReturnsNotFound()
    {
        await AuthenticateAsUser();

        var response =
            await Client.GetAsync(
                "/tags/9999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
    [Fact]
    public async Task RegularUserCanGetOfficialTagDetails()
    {
        await AuthenticateAsUser();

        var administratorId =
            Reader.Query(
                db =>
                    db.Users
                        .Single(user =>
                            user.Role ==
                            UserRole.Administrator)
                        .Id);

        var tag =
            Tag.Create(
                new Actor(
                    administratorId,
                    UserRole.Administrator),
                new TagName(
                    "Official Tag"));

        Writer.Seed(
            db =>
                db.Tags.Add(tag));

        var response =
            await Client.GetAsync(
                $"/tags/{tag.Id}");

        var result =
            await response.ReadJsonAs<GetTagDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            tag.Id,
            result.Id);

        Assert.Equal(
            "Official Tag",
            result.Name);

        Assert.Equal(
            "Official",
            result.Source);
    }

    [Fact]
    public async Task RegularUserCanGetAnotherUsersDemoTagDetails()
    {
        await AuthenticateAsUser();

        var tag =
            Tag.Create(
                new Actor(
                    9999,
                    UserRole.User),
                new TagName(
                    "Shared Tag"));

        Writer.Seed(
            db =>
                db.Tags.Add(tag));

        var response =
            await Client.GetAsync(
                $"/tags/{tag.Id}");

        var result =
            await response.ReadJsonAs<GetTagDetailsResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            "Shared Tag",
            result.Name);

        Assert.Equal(
            "Demo",
            result.Source);

        Assert.Equal(
            9999,
            result.CreatedByUserId);
    }
}
