using System.Net;

using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.FeaturedContent;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Tags.DeleteTag;

public sealed class DeleteTagRelationshipTests : IntegrationTest
{
    [Fact]
    public async Task DeleteTagInUseReturnsConflict()
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

        var project =
            Project.Create(
                actor,
                new ProjectTitle(
                    "Personal Site"),
                new ProjectDescription(
                    "Portfolio"),
                1,
                new Url(
                    "https://github.com/example/site"),
                null,
                false,
                [tag]);

        Writer.Seed(
            db =>
            {
                db.Tags.Add(tag);
                db.Projects.Add(project);
            });

        var response =
            await Client.DeleteAsync(
                $"/tags/{tag.Id}");

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);

        var existing =
            Reader.Query(
                db =>
                    db.Tags.Find(tag.Id));

        Assert.NotNull(
            existing);
    }

    [Fact]
    public async Task DeleteUnusedTagSucceeds()
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
                new TagName("Unused"));

        Writer.Seed(
            db =>
                db.Tags.Add(tag));

        var response =
            await Client.DeleteAsync(
                $"/tags/{tag.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var existing =
            Reader.Query(
                db =>
                    db.Tags.Find(tag.Id));

        Assert.Null(
            existing);
    }

    [Fact]
    public async Task DeleteTagUsedByFeaturedContentReturnsConflict()
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
            await Client.DeleteAsync(
                $"/tags/{tag.Id}");

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);

        var existing =
            Reader.Query(
                db =>
                    db.Tags.Find(tag.Id));

        Assert.NotNull(
            existing);
    }

    [Fact]
    public async Task DeleteProjectDoesNotDeleteItsTags()
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

        var project =
            Project.Create(
                actor,
                new ProjectTitle(
                    "Personal Site"),
                new ProjectDescription(
                    "Portfolio"),
                1,
                new Url(
                    "https://github.com/example/site"),
                null,
                false,
                [tag]);

        Writer.Seed(
            db =>
            {
                db.Tags.Add(tag);
                db.Projects.Add(project);
            });

        var response =
            await Client.DeleteAsync(
                $"/projects/{project.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var existingTag =
            Reader.Query(
                db =>
                    db.Tags.Find(tag.Id));

        Assert.NotNull(
            existingTag);
    }
}
