using Microsoft.AspNetCore.Identity;
using PersonalSite.Api.Application.Auth.Login;
using PersonalSite.Api.Domain.Users;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace PersonalSite.Api.Tests.IntegrationTests.Helpers;

public abstract class IntegrationTest : IDisposable
{
    private readonly CustomWebApplicationFactory factory = new();

    protected HttpClient Client { get; }

    protected EfReader Reader { get; }

    protected EfWriter Writer { get; }

    protected IntegrationTest()
    {
        Client = factory.CreateClient();
        Reader = factory.GetReader();
        Writer = factory.GetWriter();
    }

    public void Dispose()
    {
        Client.Dispose();
        factory.Dispose();
    }

    protected void SeedUser(
      string password = "analytical-engine-password")
    {
        var user = new User
        {
            Name = new UserName("Ada Lovelace"),
            Email = new UserEmail("ada@example.com"),
            PasswordHash = string.Empty
        };

        var passwordHasher = new PasswordHasher<User>();

        user.PasswordHash =
            passwordHasher.HashPassword(user, password);

        Writer.Seed(db => db.Users.Add(user));
    }

    protected async Task<int> AuthenticateAsUser(
     UserRole role = UserRole.User,
     string name = "Ada Lovelace",
     string email = "ada@example.com",
     string password = "analytical-engine-password")
    {
        var user =
            new User
            {
                Name = new UserName(name),
                Email = new UserEmail(email),
                PasswordHash = string.Empty,
                Role = role
            };

        var passwordHasher =
            new PasswordHasher<User>();

        user.PasswordHash =
            passwordHasher.HashPassword(
                user,
                password);

        Writer.Seed(db =>
            db.Users.Add(user));

        var request =
            new LoginRequest
            {
                Email = email,
                Password = password
            };

        var response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                request);

        var login =
            await response.ReadJsonAs<LoginResponse>(
                HttpStatusCode.OK);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        return user.Id;
    }
}