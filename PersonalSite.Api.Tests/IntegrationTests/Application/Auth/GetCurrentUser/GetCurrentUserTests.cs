using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PersonalSite.Api.Application.Auth.GetCurrentUser;
using PersonalSite.Api.Application.Auth.Login;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Auth.GetCurrentUser;

public class GetCurrentUserTests : IntegrationTest
{
    [Fact]
    public async Task GetCurrentUserRequiresAuthentication()
    {
        var response = await Client.GetAsync("/auth/me");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task GetCurrentUserReturnsRole()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        var response =
            await Client.GetAsync("/auth/me");

        var user =
            await response
                .ReadJsonAs<CurrentUserResponse>(
                    HttpStatusCode.OK);

        Assert.Equal(
            "Administrator",
            user.Role);
    }
    [Fact]
    public async Task GetCurrentUserReturnsTokenClaims()
    {
        SeedUser();

        var loginRequest =
            new LoginRequest
            {
                Email = "ada@example.com",
                Password = "analytical-engine-password"
            };

        var loginResponse =
            await Client.PostAsJsonAsync(
                "/auth/login",
                loginRequest);

        var login =
            await loginResponse.ReadJsonAs<LoginResponse>(
                HttpStatusCode.OK);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        var response = await Client.GetAsync("/auth/me");

        var user =
            await response.ReadJsonAs<CurrentUserResponse>(
                HttpStatusCode.OK);

        Assert.Equal(1, user.Id);
        Assert.Equal("Ada Lovelace", user.Name);
        Assert.Equal("ada@example.com", user.Email);
    }

    [Fact]
    public async Task GetCurrentUserRejectsInvalidToken()
    {
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "this-is-not-a-valid-token");

        var response = await Client.GetAsync("/auth/me");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }
}