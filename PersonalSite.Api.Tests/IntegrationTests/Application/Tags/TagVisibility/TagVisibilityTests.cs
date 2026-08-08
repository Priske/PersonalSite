using System.Net;

using PersonalSite.Api.Application.Tags.GetTagSummaries;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Tags.TagVisibility;

public sealed class TagVisibilityTests : IntegrationTest
{
    [Fact]
    public async Task RegularUserCanSeeOfficialTags()
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

        var administrator =
            new Actor(
                administratorId,
                UserRole.Administrator);

        SeedTag(
            administrator,
            "Official Tag");

        var response =
            await Client.GetAsync(
                "/tags");

        var result =
            await response.ReadJsonAs<GetTagSummariesResponse>(
                HttpStatusCode.OK);

        var tag =
            Assert.Single(
                result.Items,
                tag =>
                    tag.Name ==
                    "Official Tag");

        Assert.Equal(
            "Official",
            tag.Source);

        Assert.Equal(
            administratorId,
            tag.CreatedByUserId);
    }

    [Fact]
    public async Task RegularUserCanSeeAnotherUsersDemoTags()
    {
        await AuthenticateAsUser();

        var otherUser =
            new Actor(
                9999,
                UserRole.User);

        SeedTag(
            otherUser,
            "Other Users Tag");

        var response =
            await Client.GetAsync(
                "/tags");

        var result =
            await response.ReadJsonAs<GetTagSummariesResponse>(
                HttpStatusCode.OK);

        var tag =
            Assert.Single(
                result.Items,
                tag =>
                    tag.Name ==
                    "Other Users Tag");

        Assert.Equal(
            "Demo",
            tag.Source);

        Assert.Equal(
            9999,
            tag.CreatedByUserId);
    }

    [Fact]
    public async Task AdministratorCanSeeDemoTagsCreatedByUsers()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var user =
            new Actor(
                9999,
                UserRole.User);

        SeedTag(
            user,
            "User Created Tag");

        var response =
            await Client.GetAsync(
                "/tags");

        var result =
            await response.ReadJsonAs<GetTagSummariesResponse>(
                HttpStatusCode.OK);

        Assert.Contains(
            result.Items,
            tag =>
                tag.Name ==
                "User Created Tag");
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
            db =>
                db.Tags.Add(tag));
    }
}