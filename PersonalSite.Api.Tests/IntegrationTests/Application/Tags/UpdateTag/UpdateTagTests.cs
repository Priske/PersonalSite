using System.Net;
using System.Net.Http.Json;

using PersonalSite.Api.Application.Tags.UpdateTags;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Tags.UpdateTag;

public sealed class UpdateTagTests : IntegrationTest
{
    [Fact]
    public async Task AdministratorCanUpdateOfficialTag()
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

        var request =
            new UpdateTagRequest
            {
                Id = tag.Id,
                Name = ".NET"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/tags/{tag.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updated =
            Reader.Query(
                db => db.Tags.Find(tag.Id));

        Assert.NotNull(updated);

        Assert.Equal(
            ".NET",
            updated.Name.Value);

        Assert.Equal(
            userId,
            updated.Edited.UserId);
    }

    [Fact]
    public async Task RegularUserCanUpdateOwnDemoTag()
    {
        var userId =
            await AuthenticateAsUser();

        var tag =
            SeedTag(
                new Actor(
                    userId,
                    UserRole.User),
                "Old Name");

        var request =
            new UpdateTagRequest
            {
                Id = tag.Id,
                Name = "New Name"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/tags/{tag.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var updated =
            Reader.Query(
                db => db.Tags.Find(tag.Id));

        Assert.NotNull(updated);

        Assert.Equal(
            "New Name",
            updated.Name.Value);
    }

    [Fact]
    public async Task RegularUserCannotUpdateOfficialTag()
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
                "C#");

        var request =
            new UpdateTagRequest
            {
                Id = tag.Id,
                Name = ".NET"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/tags/{tag.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);

        var unchanged =
            Reader.Query(
                db => db.Tags.Find(tag.Id));

        Assert.NotNull(unchanged);

        Assert.Equal(
            "C#",
            unchanged.Name.Value);
    }

    [Fact]
    public async Task RegularUserCannotUpdateAnotherUsersDemoTag()
    {
        await AuthenticateAsUser();

        var tag =
            SeedTag(
                new Actor(
                    9999,
                    UserRole.User),
                "Other User Tag");

        var request =
            new UpdateTagRequest
            {
                Id = tag.Id,
                Name = "Changed"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/tags/{tag.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateTagToExistingNameReturnsBadRequest()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        var first =
            SeedTag(
                actor,
                "C#");

        SeedTag(
            actor,
            "React");

        var request =
            new UpdateTagRequest
            {
                Id = first.Id,
                Name = "React"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/tags/{first.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateTagWithWhitespaceNameReturnsBadRequest()
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

        var request =
            new UpdateTagRequest
            {
                Id = tag.Id,
                Name = "   "
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/tags/{tag.Id}",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateUnknownTagReturnsNotFound()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var request =
            new UpdateTagRequest
            {
                Id = 9999,
                Name = "C#"
            };

        var response =
            await Client.PutAsJsonAsync(
                "/tags/9999",
                request);

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