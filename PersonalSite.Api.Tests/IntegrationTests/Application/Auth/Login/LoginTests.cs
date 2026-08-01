using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using PersonalSite.Api.Application.Auth.Login;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;



namespace PersonalSite.Api.Tests.IntegrationTests.Application.Auth.Login;

public class LoginTestTests : IntegrationTest
{


    [Fact]
    public async Task LoginReturnsAccessToken()
    {
        SeedUser();

        var request =
            new LoginRequest
            {
                Email = "ada@example.com",
                Password = "analytical-engine-password"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                request);

        var login =
            await response.ReadJsonAs<LoginResponse>(
                HttpStatusCode.OK);

        Assert.False(
            string.IsNullOrWhiteSpace(login.AccessToken));

        Assert.True(login.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginNormalizesEmail()
    {
        SeedUser();

        var request =
            new LoginRequest
            {
                Email = "  ADA@EXAMPLE.COM  ",
                Password = "analytical-engine-password"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                request);

        await response.ShouldHaveStatusCode(HttpStatusCode.OK);

        var content =
            await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(content);

        var jwtToken = new JwtSecurityTokenHandler()
            .ReadJwtToken(content.AccessToken);

        var id = jwtToken.Claims.Single(
            claim => claim.Type == ClaimTypes.NameIdentifier).Value;

        Assert.Equal("2", id);

        var role = jwtToken.Claims.Single(
            claim => claim.Type == ClaimTypes.Role).Value;

        Assert.Equal(nameof(UserRole.User), role);
    }
    [Fact]
    public async Task LoginReturnsUnauthorizedForWrongPassword()
    {
        SeedUser();

        var request =
            new LoginRequest
            {
                Email = "ada@example.com",
                Password = "wrong-password"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LoginReturnsUnauthorizedForUnknownEmail()
    {
        SeedUser();

        var request =
            new LoginRequest
            {
                Email = "unknown@example.com",
                Password = "analytical-engine-password"
            };

        var response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }
}

