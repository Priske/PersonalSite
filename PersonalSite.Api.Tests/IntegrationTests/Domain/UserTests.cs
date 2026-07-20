using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Tests.IntegrationTests.Domain;

public sealed class UserTests
{
    [Fact]
    public void NewUser_ShouldHaveDefaultValues()
    {
        var user = new User
        {
            Name = new UserName("Ben"),
            Email = new UserEmail("ben@example.com")
        };

        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void User_ShouldStoreProvidedValues()
    {
        var name = new UserName("Administrator");
        var email = new UserEmail("admin@example.com");

        var user = new User
        {
            Id = 42,
            Name = name,
            Email = email,
            PasswordHash = "hashed-password",
            Role = UserRole.Administrator
        };

        Assert.Equal(42, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.Equal(UserRole.Administrator, user.Role);
    }

    [Fact]
    public void Role_ShouldBeChangeable()
    {
        var user = new User
        {
            Name = new UserName("Ben"),
            Email = new UserEmail("ben@example.com")
        };

        user.Role = UserRole.Administrator;

        Assert.Equal(UserRole.Administrator, user.Role);
    }

    [Fact]
    public void PasswordHash_ShouldBeChangeable()
    {
        var user = new User
        {
            Name = new UserName("Ben"),
            Email = new UserEmail("ben@example.com")
        };

        user.PasswordHash = "new-password-hash";

        Assert.Equal("new-password-hash", user.PasswordHash);
    }
}