using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Tests.Domain.Tags;

public sealed class TagTests
{
    [Fact]
    public void Create_AsAdministrator_CreatesOfficialTag()
    {
        var actor =
            new Actor(
                1,
                UserRole.Administrator);

        var before =
            DateTimeOffset.UtcNow;

        var tag =
            Tag.Create(
                actor,
                new TagName("C#"));

        var after =
            DateTimeOffset.UtcNow;

        Assert.Equal(
            "C#",
            tag.Name.Value);

        Assert.Equal(
            ContentSource.Official,
            tag.Source);

        Assert.Equal(
            actor.UserId,
            tag.Created.UserId);

        Assert.Equal(
            actor.UserId,
            tag.Edited.UserId);

        Assert.InRange(
            tag.Created.At,
            before,
            after);

        Assert.Equal(
            tag.Created.At,
            tag.Edited.At);

        Assert.Empty(
            tag.Projects);
    }

    [Fact]
    public void Create_AsRegularUser_CreatesDemoTag()
    {
        var actor =
            new Actor(
                5,
                UserRole.User);

        var tag =
            Tag.Create(
                actor,
                new TagName("React"));

        Assert.Equal(
            ContentSource.Demo,
            tag.Source);

        Assert.Equal(
            actor.UserId,
            tag.Created.UserId);

        Assert.Equal(
            actor.UserId,
            tag.Edited.UserId);
    }
}