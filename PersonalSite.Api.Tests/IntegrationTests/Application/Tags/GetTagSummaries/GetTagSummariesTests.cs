using System.Net;

using PersonalSite.Api.Application.Tags.GetTagSummaries;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Tags.GetTagSummaries;

public sealed class GetTagSummariesTests : IntegrationTest
{
    [Fact]
    public async Task GetTagsReturnsTags()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        SeedTag(
            actor,
            "C#");

        SeedTag(
            actor,
            "React");

        var response =
            await Client.GetAsync("/tags");

        var result =
            await response.ReadJsonAs<GetTagSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            2,
            result.TotalItems);

        Assert.Equal(
            1,
            result.Page);

        Assert.Equal(
            10,
            result.PageSize);

        Assert.Equal(
            1,
            result.TotalPages);

        Assert.Equal(
            2,
            result.Items.Count);

        Assert.Equal(
            "C#",
            result.Items[0].Name);

        Assert.Equal(
            "React",
            result.Items[1].Name);
    }

    [Fact]
    public async Task GetTagsCanSearchByName()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        SeedTag(
            actor,
            "ASP.NET Core");

        SeedTag(
            actor,
            "React");

        var response =
            await Client.GetAsync(
                "/tags?search=ASP");

        var result =
            await response.ReadJsonAs<GetTagSummariesResponse>(
                HttpStatusCode.OK);

        var tag =
            Assert.Single(
                result.Items);

        Assert.Equal(
            "ASP.NET Core",
            tag.Name);

        Assert.Equal(
            1,
            result.TotalItems);
    }

    [Fact]
    public async Task GetTagsUsesRequestedPage()
    {
        var userId =
            await AuthenticateAsUser(
                UserRole.Administrator);

        var actor =
            new Actor(
                userId,
                UserRole.Administrator);

        SeedTag(actor, "C#");
        SeedTag(actor, "React");
        SeedTag(actor, "TypeScript");

        var response =
            await Client.GetAsync(
                "/tags?page=2&pageSize=1");

        var result =
            await response.ReadJsonAs<GetTagSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Single(
            result.Items);

        Assert.Equal(
            "React",
            result.Items[0].Name);

        Assert.Equal(
            2,
            result.Page);

        Assert.Equal(
            1,
            result.PageSize);

        Assert.Equal(
            3,
            result.TotalItems);

        Assert.Equal(
            3,
            result.TotalPages);
    }

    [Fact]
    public async Task GetTagsClampsInvalidPagingValues()
    {
        await AuthenticateAsUser();

        var response =
            await Client.GetAsync(
                "/tags?page=-5&pageSize=1000");

        var result =
            await response.ReadJsonAs<GetTagSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Equal(
            1,
            result.Page);

        Assert.Equal(
            50,
            result.PageSize);
    }

    [Fact]
    public async Task GetTagsWithoutAuthenticationReturnsUnauthorized()
    {
        var response =
            await Client.GetAsync("/tags");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    private void SeedTag(
        Actor actor,
        string name)
    {
        var tag =
            Tag.Create(
                actor,
                new TagName(name));

        Writer.Seed(
            db => db.Tags.Add(tag));
    }
}