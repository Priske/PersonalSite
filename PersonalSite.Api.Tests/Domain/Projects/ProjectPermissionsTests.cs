using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Tests.Domain.Projects;

public sealed class ProjectPermissionsTests
{
    [Fact]
    public void EnsureCanManage_AdministratorCanManageOfficialProject()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var project =
            CreateProject(
                administrator);

        var exception =
            Record.Exception(
                () =>
                    ProjectPermissions.EnsureCanManage(
                        administrator,
                        project));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanManage_AdministratorCanManageDemoProject()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var user =
            new Actor(
                2,
                UserRole.User);

        var project =
            CreateProject(
                user);

        var exception =
            Record.Exception(
                () =>
                    ProjectPermissions.EnsureCanManage(
                        administrator,
                        project));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanManage_UserCanManageOwnDemoProject()
    {
        var user =
            new Actor(
                2,
                UserRole.User);

        var project =
            CreateProject(
                user);

        var exception =
            Record.Exception(
                () =>
                    ProjectPermissions.EnsureCanManage(
                        user,
                        project));

        Assert.Null(
            exception);
    }

    [Fact]
    public void EnsureCanManage_UserCannotManageOfficialProject()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var user =
            new Actor(
                2,
                UserRole.User);

        var project =
            CreateProject(
                administrator);

        Assert.Throws<ForbiddenOperationException>(
            () =>
                ProjectPermissions.EnsureCanManage(
                    user,
                    project));
    }

    [Fact]
    public void EnsureCanManage_UserCannotManageAnotherUsersDemoProject()
    {
        var owner =
            new Actor(
                2,
                UserRole.User);

        var otherUser =
            new Actor(
                3,
                UserRole.User);

        var project =
            CreateProject(
                owner);

        Assert.Throws<ForbiddenOperationException>(
            () =>
                ProjectPermissions.EnsureCanManage(
                    otherUser,
                    project));
    }

    [Theory]
    [InlineData(UserRole.Administrator)]
    [InlineData(UserRole.User)]
    public void EnsureCanCreate_AdministratorAndUserAreAllowed(
        UserRole role)
    {
        var actor =
            new Actor(
                1,
                role);

        var exception =
            Record.Exception(
                () =>
                    ProjectPermissions.EnsureCanCreate(
                        actor));

        Assert.Null(
            exception);
    }

    [Theory]
    [InlineData(UserRole.Administrator)]
    [InlineData(UserRole.User)]
    public void EnsureCanViewDirectory_AdministratorAndUserAreAllowed(
        UserRole role)
    {
        var actor =
            new Actor(
                1,
                role);

        var exception =
            Record.Exception(
                () =>
                    ProjectPermissions.EnsureCanViewDirectory(
                        actor));

        Assert.Null(
            exception);
    }

    private static Project CreateProject(
        Actor actor)
    {
        return Project.Create(
            actor,
            new ProjectTitle("Project"),
            new ProjectDescription(
                "Project description"),
            1,
            new Url(
                "https://github.com/example/project"),
            null,
            false,
            []);
    }
}