using System.Net;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Users.DeleteUser;

public class DeleteUserTests : IntegrationTest
{
    [Fact]
    public async Task DeleteUserRemovesFakeUser()
    {
        await AuthenticateAsUser(
            UserRole.User);

        var fakeUser =
            new User
            {
                Name =
                    new UserName(
                        "Fake User"),
                Email =
                    new UserEmail(
                        "fake@example.com"),
                Role =
                    UserRole.FakeUser
            };

        Writer.Seed(
            db =>
                db.Users.Add(fakeUser));

        var response =
            await Client.DeleteAsync(
                $"/users/{fakeUser.Id}");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);

        var deletedUser =
            Reader.Query(
                db =>
                    db.Users.Find(
                        fakeUser.Id));

        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUserReturnsUnauthorizedWhenNotLoggedIn()
    {
        var response =
            await Client.DeleteAsync(
                "/users/9999");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegularUserCannotDeleteRealUser()
    {
        await AuthenticateAsUser(
            UserRole.User);

        var targetUser =
            new User
            {
                Name =
                    new UserName(
                        "Target User"),
                Email =
                    new UserEmail(
                        "target@example.com"),
                Role =
                    UserRole.User
            };

        Writer.Seed(
            db =>
                db.Users.Add(targetUser));

        var response =
            await Client.DeleteAsync(
                $"/users/{targetUser.Id}");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var existingUser =
            Reader.Query(
                db =>
                    db.Users.Find(
                        targetUser.Id));

        Assert.NotNull(existingUser);
    }

    [Fact]
    public async Task AdministratorCanDeleteRealUser()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var targetUser =
            new User
            {
                Name =
                    new UserName(
                        "Target User"),
                Email =
                    new UserEmail(
                        "target@example.com"),
                Role =
                    UserRole.User
            };

        Writer.Seed(
            db =>
                db.Users.Add(targetUser));

        var response =
            await Client.DeleteAsync(
                $"/users/{targetUser.Id}");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);

        var deletedUser =
            Reader.Query(
                db =>
                    db.Users.Find(
                        targetUser.Id));

        Assert.Null(deletedUser);
    }
}