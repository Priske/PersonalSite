using System.Net;
using System.Net.Http.Json;
using PersonalSite.Api.Application.Users.UpdateUsers;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Users.UpdateUsers;

public class UpdateUserTests : IntegrationTest

{

    [Fact]
    public async Task PutUserUpdatesUser()
    {
        var userId = await AuthenticateAsUser();

        var request =
            new UpdateUserRequest
            {
                Name = "Friedrich",
                Email = "friedrich@engels.de"
            };

        var response = await Client.PutAsJsonAsync($"/users/{userId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var user = Reader.Query(db => db.Users.Find(2));

        Assert.NotNull(user);
        Assert.Equal("Friedrich", user.Name);
        Assert.Equal("friedrich@engels.de", user.Email);
    }

    [Fact]
    public async Task PutUserReturnsForbiddenUpdatingOtherId()
    {
        await AuthenticateAsUser();
        var request =
            new UpdateUserRequest
            {
                Name = "Unknown User",
                Email = "Unknown@email.com",
            };

        var response = await Client.PutAsJsonAsync("/users/9999", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

    }

    [Fact]
    public async Task PutUserReturnsUnautharizedDeleting()
    {
        var request =
            new UpdateUserRequest
            {
                Name = "Unknown User",
                Email = "Unknown@email",
            };

        var response = await Client.PutAsJsonAsync("/users/9999", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

    }


    [Fact]

    public async Task PutUserRejectsUpdatedUserWithExistingEmail()
    {
        var userId = await AuthenticateAsUser();

        Writer.Seed(db =>
        {
            db.Users.AddRange(
                            new User
                            {
                                Name = new UserName("Karl"),
                                Email = new UserEmail("karl@marx.de"),
                                PasswordHash = ""
                            },
                            new User
                            {
                                Name = new UserName("Fried"),
                                Email = new UserEmail("friedrich@engels.de"),
                                PasswordHash = ""
                            }
                            );
        });

        var request =
            new UpdateUserRequest
            {
                Name = "Friedrich",
                Email = "friedrich@engels.de"
            };

        var response = await Client.PutAsJsonAsync($"/users/{userId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

}