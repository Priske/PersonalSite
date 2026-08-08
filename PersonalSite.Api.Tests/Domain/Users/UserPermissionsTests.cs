using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Tests.Domain.Users;

public sealed class UserPermissionsTests
{
    [Fact]
    public void EnsureCanViewDirectory_AdministratorIsAllowed()
    {
        var actor =
            new Actor(
                1,
                UserRole.Administrator);

        var exception =
            Record.Exception(
                () =>
                    UserPermissions.EnsureCanViewDirectory(
                        actor));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanViewDirectory_UserIsForbidden()
    {
        var actor =
            new Actor(
                1,
                UserRole.User);

        Assert.Throws<ForbiddenOperationException>(
            () =>
                UserPermissions.EnsureCanViewDirectory(
                    actor));
    }

    [Fact]
    public void EnsureCanManage_AdministratorCanManageAnotherUser()
    {
        var actor =
            new Actor(
                1,
                UserRole.Administrator);

        var exception =
            Record.Exception(
                () =>
                    UserPermissions.EnsureCanManage(
                        actor,
                        999));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanManage_UserCanManageSelf()
    {
        var actor =
            new Actor(
                5,
                UserRole.User);

        var exception =
            Record.Exception(
                () =>
                    UserPermissions.EnsureCanManage(
                        actor,
                        5));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanManage_UserCannotManageAnotherUser()
    {
        var actor =
            new Actor(
                5,
                UserRole.User);

        Assert.Throws<ForbiddenOperationException>(
            () =>
                UserPermissions.EnsureCanManage(
                    actor,
                    6));
    }

    [Fact]
    public void EnsureCanDelete_AdministratorCanDeleteRegularUser()
    {
        var actor =
            new Actor(
                1,
                UserRole.Administrator);

        var target =
            CreateUser(
                UserRole.User);

        var exception =
            Record.Exception(
                () =>
                    UserPermissions.EnsureCanDelete(
                        actor,
                        target));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanDelete_UserCanDeleteFakeUser()
    {
        var actor =
            new Actor(
                5,
                UserRole.User);

        var target =
            CreateUser(
                UserRole.FakeUser);

        var exception =
            Record.Exception(
                () =>
                    UserPermissions.EnsureCanDelete(
                        actor,
                        target));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanDelete_UserCannotDeleteRegularUser()
    {
        var actor =
            new Actor(
                5,
                UserRole.User);

        var target =
            CreateUser(
                UserRole.User);

        Assert.Throws<ForbiddenOperationException>(
            () =>
                UserPermissions.EnsureCanDelete(
                    actor,
                    target));
    }

    private static User CreateUser(
        UserRole role)
    {
        return new User
        {
            Name =
                new UserName(
                    "Test User"),

            Email =
                new UserEmail(
                    $"{role.ToString().ToLowerInvariant()}@example.com"),

            Role =
                role
        };
    }
}