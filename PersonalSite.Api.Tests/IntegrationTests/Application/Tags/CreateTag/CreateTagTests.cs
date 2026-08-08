using System.Net;
using System.Net.Http.Json;

using PersonalSite.Api.Application.Tags.CreateTag;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Tags.CreateTag;

public sealed class CreateTagTests : IntegrationTest
{
    [Fact]
    public async Task PostTagCreatesOfficialTagAsAdministrator()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var request =
            new CreateTagRequest
            {
                Name = "ASP.NET Core"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/tags",
                request);

        var created =
            await response.ReadJsonAs<CreateTagResponse>(
                HttpStatusCode.Created);

        Assert.True(created.Id > 0);

        Assert.Equal(
            "ASP.NET Core",
            created.Name);

        Assert.Equal(
            ContentSource.Official.ToString(),
            created.Source);

        Assert.Equal(
            userId,
            created.CreatedByUserId);

        Assert.Equal(
            userId,
            created.LastEditedByUserId);

        var tag =
            Reader.Query(
                db => db.Tags.Find(created.Id));

        Assert.NotNull(tag);

        Assert.Equal(
            "ASP.NET Core",
            tag.Name.Value);

        Assert.Equal(
            ContentSource.Official,
            tag.Source);

        Assert.Equal(
            userId,
            tag.Created.UserId);

        Assert.Equal(
            userId,
            tag.Edited.UserId);
    }

    [Fact]
    public async Task PostTagCreatesDemoTagAsRegularUser()
    {
        var userId =
            await AuthenticateAsUser();

        var request =
            new CreateTagRequest
            {
                Name = "React"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/tags",
                request);

        var created =
            await response.ReadJsonAs<CreateTagResponse>(
                HttpStatusCode.Created);

        Assert.Equal(
            ContentSource.Demo.ToString(),
            created.Source);

        Assert.Equal(
            userId,
            created.CreatedByUserId);

        var tag =
            Reader.Query(
                db => db.Tags.Find(created.Id));

        Assert.NotNull(tag);

        Assert.Equal(
            ContentSource.Demo,
            tag.Source);

        Assert.Equal(
            userId,
            tag.Created.UserId);
    }

    [Fact]
    public async Task PostDuplicateTagReturnsBadRequest()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var request =
            new CreateTagRequest
            {
                Name = "C#"
            };

        await Client.PostAsJsonAsync(
            "/tags",
            request);

        var response =
            await Client.PostAsJsonAsync(
                "/tags",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task PostTagWithWhitespaceNameReturnsBadRequest()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var request =
            new CreateTagRequest
            {
                Name = "   "
            };

        var response =
            await Client.PostAsJsonAsync(
                "/tags",
                request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task PostTagWithoutAuthenticationReturnsUnauthorized()
    {
        var request =
            new CreateTagRequest
            {
                Name = "C#"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/tags",
                request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}