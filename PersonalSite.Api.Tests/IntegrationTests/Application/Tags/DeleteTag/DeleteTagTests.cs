using System.Net;

using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Tags.DeleteTag;

public sealed class DeleteTagTests : IntegrationTest
{
    [Fact]
    public async Task AdministratorCanDeleteOfficialTag()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var tag =
            SeedTag(
                new Actor(
                    userId,
                    UserRole.Administrator),
                "C#");

        var response =
            await Client.DeleteAsync(
                $"/tags/{tag.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var deleted =
            Reader.Query(
                db => db.Tags.Find(tag.Id));

        Assert.Null(deleted);
    }

    [Fact]
    public async Task RegularUserCanDeleteOwnDemoTag()
    {
        var userId =
            await AuthenticateAsUser();

        var tag =
            SeedTag(
                new Actor(
                    userId,
                    UserRole.User),
                "My Tag");

        var response =
            await Client.DeleteAsync(
                $"/tags/{tag.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var deleted =
            Reader.Query(
                db => db.Tags.Find(tag.Id));

        Assert.Null(deleted);
    }

    [Fact]
    public async Task RegularUserCannotDeleteOfficialTag()
    {
        await AuthenticateAsUser();

        var administratorId =
            Reader.Query(
                db =>
                    db.Users
                        .Single(
                            user =>
                                user.Role ==
                                UserRole.Administrator)
                        .Id);

        var tag =
            SeedTag(
                new Actor(
                    administratorId,
                    UserRole.Administrator),
                "Official Tag");

        var response =
            await Client.DeleteAsync(
                $"/tags/{tag.Id}");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);

        var existing =
            Reader.Query(
                db => db.Tags.Find(tag.Id));

        Assert.NotNull(existing);
    }

    [Fact]
    public async Task RegularUserCannotDeleteAnotherUsersDemoTag()
    {
        await AuthenticateAsUser();

        var tag =
            SeedTag(
                new Actor(
                    9999,
                    UserRole.User),
                "Other Tag");

        var response =
            await Client.DeleteAsync(
                $"/tags/{tag.Id}");

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteUnknownTagReturnsNotFound()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var response =
            await Client.DeleteAsync(
                "/tags/9999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    private Tag SeedTag(
        Actor actor,
        string name)
    {
        var tag =
            Tag.Create(
                actor,
                new TagName(name));

        Writer.Seed(
            db => db.Tags.Add(tag));

        return tag;
    }
}