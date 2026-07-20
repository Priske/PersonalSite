using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Identity;
using PersonalSite.Api.Application.Users.CreateUsers;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Users.CreateUsers;

public class CreatUserTests : IntegrationTest
{

    [Fact]
    public async Task PostUserCreatesUser()
    {
        var request =
            new CreateUserRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
                Password = "adminadminadminadmin"
            };
        var response = await Client.PostAsJsonAsync("/users", request);
        var created = await response.ReadJsonAs<CreateUserResponse>(HttpStatusCode.Created);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("For Petes Sake", created.Name);
        //
        // Reader Usage Test
        //
        var User = Reader.Query(context => context.Find<User>(created.Id));

        Assert.NotNull(User);
        Assert.Equal("pp@pepe.com", User.Email);
        Assert.Equal("For Petes Sake", User.Name);
    }


    [Fact]
    public async Task PostUserWhitespaceReturnsBadRequest()
    {
        var request = new CreateUserRequest
        {
            Name = "    ",
            Email = "PP@PePe.com",
            Password = "adminadminadminadmin"
        };

        var response = await Client.PostAsJsonAsync("/users", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostUserInvalidEmailReturnsBadRequest()
    {
        var request = new CreateUserRequest
        {
            Name = "For Petes Sake",
            Email = "PPPePe.com",
            Password = "adminadminadminadmin"
        };

        var response = await Client.PostAsJsonAsync("/users", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UserEmailRejectsDuplicateEmail()
    {

        var request = new CreateUserRequest
        {
            Name = "For Petes Sake",
            Email = "PPP@ePe.com",
            Password = "adminadminadminadmin"
        };

        var response1 = await Client.PostAsJsonAsync("/users", request);

        var request2 = new CreateUserRequest
        {
            Name = "For Petes Sake",
            Email = "PPP@ePe.com",
            Password = "adminadmin"
        };

        var response2 = await Client.PostAsJsonAsync("/users", request2);

        // First request should succeed
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Second request should fail due to duplicate email
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }


    [Fact]
    public async Task UserPasswordGetsHashed()
    {
        var request =
            new CreateUserRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
                Password = "adminadminadminadmin"
            };

        var response = await Client.PostAsJsonAsync("/users", request);
        var created = await response.ReadJsonAs<CreateUserResponse>(HttpStatusCode.Created);
        var user = Reader.Query(db =>
            db.Users.Single(current => current.Id == created.Id));
        Assert.NotEqual("analytical-engine-password", user.PasswordHash);

        var passwordHasher = new PasswordHasher<User>();

        var result = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            "adminadminadminadmin");

        Assert.Equal(PasswordVerificationResult.Success, result);

    }

    [Fact]
    public async Task UserPasswordRejectsEmtpy()
    {
        var request =
            new CreateUserRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
                Password = ""
            };
        var response = await Client.PostAsJsonAsync("/users", request);


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task UserPasswordRejectsTooShort()
    {
        var request =
            new CreateUserRequest
            {
                Name = "For Petes Sake",
                Email = "PP@PePe.com",
                Password = "1234"
            };
        var response = await Client.PostAsJsonAsync("/users", request);


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUserCreatesRegularUser()
    {
        var request =
            new CreateUserRequest
            {
                Name = "Grace Hopper",
                Email = "grace@example.com",
                Password = "debugging-moth-password"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/users",
                request);

        var created =
            await response
                .ReadJsonAs<CreateUserResponse>(
                    HttpStatusCode.Created);

        var user =
            Reader.Query(db =>
                db.Users.Find(created.Id));

        Assert.NotNull(user);

        Assert.Equal(
            UserRole.User,
            user.Role);
    }
}