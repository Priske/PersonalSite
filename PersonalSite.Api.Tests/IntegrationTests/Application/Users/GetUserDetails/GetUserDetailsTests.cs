using System.Net;
using PersonalSite.Api.Application.Users.GetUserDetails;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;


namespace PersonalSite.Api.Tests.IntegrationTests.Application.Users.GetUserDetails;

public class GetUserDetails : IntegrationTest
{
    [Fact]
    public async Task GetUserDetailsReturnsBook()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);
        Writer.Seed(db =>
        {
            db.Users.Add(
                new User
                {
                    Name = new UserName("Dune"),
                    Email = new UserEmail("Frank@Herbert.com"),
                });
        });

        var response = await Client.GetAsync("/users/2");
        var user = await response.ReadJsonAs<GetUserDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(user);
        Assert.Equal(2, user.Id);
        Assert.Equal("Dune", user.Name);
        Assert.Equal("frank@herbert.com", user.Email);
    }

    [Fact]
    public async Task GetUserDetailsReturnsNotFoundWhenUserDoesNotExist()
    {
        await AuthenticateAsUser(UserRole.Administrator);
        var response = await Client.GetAsync("/users/9999");
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}