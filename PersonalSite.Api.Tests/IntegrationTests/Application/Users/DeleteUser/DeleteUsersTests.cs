using System.Net;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;


namespace PersonalSite.Api.Tests.IntegrationTests.Application.Users.DeleteUser;

public class DeleteUserTests : IntegrationTest
{
    [Fact]
    public async Task DeleteUserRemovesUser()
    {

        var userId = await AuthenticateAsUser();
        var response = await Client.DeleteAsync($"/users/{userId}");

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var user = Reader.Query(db => db.Users.Find(2));

        Assert.Null(user);
    }

    [Fact]
    public async Task DeleteUserReturnsUnauthorizedDeletingNonLoggedInUser()
    {
        var response = await Client.DeleteAsync("/users/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteUserReturnsForbiddenDeletingNonLoggedInUser()
    {
        await AuthenticateAsUser();
        var response = await Client.DeleteAsync("/users/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

}