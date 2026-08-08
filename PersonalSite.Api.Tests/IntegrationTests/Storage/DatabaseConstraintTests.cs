using Microsoft.EntityFrameworkCore;

using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Storage;

public sealed class DatabaseConstraintTests : IntegrationTest
{
    [Fact]
    public void UserEmailMustBeUnique()
    {
        var first =
            new User
            {
                Name =
                    new UserName(
                        "First User"),

                Email =
                    new UserEmail(
                        "same@example.com")
            };

        var second =
            new User
            {
                Name =
                    new UserName(
                        "Second User"),

                Email =
                    new UserEmail(
                        "same@example.com")
            };

        Assert.Throws<DbUpdateException>(
            () =>
                Writer.Seed(
                    db =>
                        db.Users.AddRange(
                            first,
                            second)));
    }

    [Fact]
    public void TagNameMustBeGloballyUnique()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var regularUser =
            new Actor(
                999,
                UserRole.User);

        var officialTag =
            Tag.Create(
                administrator,
                new TagName("C#"));

        var demoTag =
            Tag.Create(
                regularUser,
                new TagName("C#"));

        Assert.Throws<DbUpdateException>(
            () =>
                Writer.Seed(
                    db =>
                        db.Tags.AddRange(
                            officialTag,
                            demoTag)));
    }

    [Fact]
    public void OfficialProjectsCannotHaveSameDisplayOrder()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var first =
            CreateProject(
                administrator,
                "First",
                1);

        var second =
            CreateProject(
                administrator,
                "Second",
                1);

        Assert.Throws<DbUpdateException>(
            () =>
                Writer.Seed(
                    db =>
                        db.Projects.AddRange(
                            first,
                            second)));
    }

    [Fact]
    public void SameUserDemoProjectsCannotHaveSameDisplayOrder()
    {
        var user =
            new Actor(
                999,
                UserRole.User);

        var first =
            CreateProject(
                user,
                "First",
                1);

        var second =
            CreateProject(
                user,
                "Second",
                1);

        Assert.Throws<DbUpdateException>(
            () =>
                Writer.Seed(
                    db =>
                        db.Projects.AddRange(
                            first,
                            second)));
    }

    [Fact]
    public void DifferentUsersDemoProjectsCanHaveSameDisplayOrder()
    {
        var firstUser =
            new Actor(
                998,
                UserRole.User);

        var secondUser =
            new Actor(
                999,
                UserRole.User);

        var first =
            CreateProject(
                firstUser,
                "First",
                1);

        var second =
            CreateProject(
                secondUser,
                "Second",
                1);

        var exception =
            Record.Exception(
                () =>
                    Writer.Seed(
                        db =>
                            db.Projects.AddRange(
                                first,
                                second)));

        Assert.Null(
            exception);
    }

    [Fact]
    public void OfficialAndDemoProjectCanHaveSameDisplayOrder()
    {
        var administrator =
            new Actor(
                1,
                UserRole.Administrator);

        var user =
            new Actor(
                999,
                UserRole.User);

        var official =
            CreateProject(
                administrator,
                "Official",
                1);

        var demo =
            CreateProject(
                user,
                "Demo",
                1);

        var exception =
            Record.Exception(
                () =>
                    Writer.Seed(
                        db =>
                            db.Projects.AddRange(
                                official,
                                demo)));

        Assert.Null(
            exception);
    }

    private static Project CreateProject(
        Actor actor,
        string title,
        int displayOrder)
    {
        return Project.Create(
            actor,
            new ProjectTitle(title),
            new ProjectDescription(
                $"{title} description"),
            displayOrder,
            new Url(
                $"https://github.com/example/{title.ToLowerInvariant()}"),
            null,
            false,
            []);
    }
}