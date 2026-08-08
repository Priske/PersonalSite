using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Tests.Domain.Tags;

public sealed class TagPermissionsTests
{
    [Fact]
    public void EnsureCanManage_AdministratorCanManageOfficialTag()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var tag =
            Tag.Create(
                administrator,
                new TagName("C#"));

        var exception =
            Record.Exception(
                () =>
                    TagPermissions.EnsureCanManage(
                        administrator,
                        tag));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanManage_AdministratorCanManageDemoTag()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var user =
            new Actor(
                2,
                UserRole.User);

        var tag =
            Tag.Create(
                user,
                new TagName("React"));

        var exception =
            Record.Exception(
                () =>
                    TagPermissions.EnsureCanManage(
                        administrator,
                        tag));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanManage_UserCanManageOwnDemoTag()
    {
        var user =
            new Actor(
                2,
                UserRole.User);

        var tag =
            Tag.Create(
                user,
                new TagName("React"));

        var exception =
            Record.Exception(
                () =>
                    TagPermissions.EnsureCanManage(
                        user,
                        tag));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanManage_UserCannotManageAnotherUsersDemoTag()
    {
        var owner =
            new Actor(
                2,
                UserRole.User);

        var otherUser =
            new Actor(
                3,
                UserRole.User);

        var tag =
            Tag.Create(
                owner,
                new TagName("React"));

        var exception =
            Assert.Throws<ForbiddenOperationException>(
                () =>
                    TagPermissions.EnsureCanManage(
                        otherUser,
                        tag));

        Assert.Equal(
            "This actor cannot manage this tag.",
            exception.Message);
    }

    [Fact]
    public void EnsureCanManage_UserCannotManageOfficialTag()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var user =
            new Actor(
                2,
                UserRole.User);

        var tag =
            Tag.Create(
                administrator,
                new TagName("C#"));

        var exception =
            Assert.Throws<ForbiddenOperationException>(
                () =>
                    TagPermissions.EnsureCanManage(
                        user,
                        tag));

        Assert.Equal(
            "This actor cannot manage this tag.",
            exception.Message);
    }
}