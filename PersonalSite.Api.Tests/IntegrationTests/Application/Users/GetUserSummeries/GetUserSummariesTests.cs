using System.Net;
using System.Net.Http.Json;
using PersonalSite.Api.Application.Users.GetUserSummeries;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;



namespace PersonalSite.Api.Tests.IntegrationTests.Application.Users.GetUserSummeries;

public class GetUserSummariesTests : IntegrationTest
{

    [Fact]
    public async Task GetUserSummaries()
    {
        await AuthenticateAsUser(UserRole.Administrator);
        Writer.Seed(db => db.Users.Add(
                new User
                {
                    Name = new UserName("Cannery Row"),
                    Email = new UserEmail("John@Steinbeck.com"),
                    PasswordHash = ""
                }
            ));


        var response = await Client.GetAsync("/users");
        var result = await response.ReadJsonAs<GetUserSummariesResponse>(HttpStatusCode.OK);

        Assert.NotNull(result);

        var userSummary = Assert.Single(
                result.Items,
                user => user.Email == "john@steinbeck.com");
        Assert.Equal("john@steinbeck.com", userSummary.Email);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetUserSummariesRejectsAsUsers()
    {
        await AuthenticateAsUser(UserRole.User);
        Writer.Seed(db => db.Users.Add(
                new User
                {
                    Name = new UserName("Cannery Row"),
                    Email = new UserEmail("John@Steinbeck.com"),
                    PasswordHash = ""
                }
            ));

        var response = await Client.GetAsync("/users");
        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);

    }
    [Fact]
    public async Task GetUserSummariesReturnsRequestedPage()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        Writer.Seed(db =>
        {
            db.Users.AddRange(
                new User
                {
                    Name = new UserName("Jefke"),
                    Email = new UserEmail("Jefke@jef.jef")
                },
                new User
                {
                    Name = new UserName("Jefke 2"),
                    Email = new UserEmail("Jefke2@jef.jef")
                },
                new User
                {
                    Name = new UserName("Jefke 3"),
                    Email = new UserEmail("Jefke3@jef.jef")
                });
        });

        var result =
            await Client.GetFromJsonAsync<GetUserSummariesResponse>(
                "/users?page=2&pageSize=1");

        Assert.NotNull(result);

        var user = Assert.Single(result.Items);


        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(5, result.TotalItems);
        Assert.Equal(5, result.TotalPages);
    }

    [Fact]
    public async Task GetUserSummariesReturnsEmptyItemsWhenPageIsTooHigh()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);
        Writer.Seed(db =>
        {
            db.Users.Add(
                new User
                {
                    Name = new UserName("Jefke 3"),
                    Email = new UserEmail("Jefke3@jef.jef")
                });
        });

        var result = await Client.GetFromJsonAsync<GetUserSummariesResponse>("/users?page=99&pageSize=10");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(99, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetUserSummariesCanSearchByEmail()
    {
        await AuthenticateAsUser(UserRole.Administrator);
        Writer.Seed(db =>
        {
            db.Users.AddRange(
                new User
                {
                    Name = new UserName("Jefke 3"),
                    Email = new UserEmail("Jefke3@jef.jef")
                },
                new User
                {
                    Name = new UserName("Karl"),
                    Email = new UserEmail("Karl@Marx.de")
                });
        });

        var response = await Client.GetAsync("/users?search=Marx");

        var result = await response.ReadJsonAs<GetUserSummariesResponse>(HttpStatusCode.OK);

        var user = Assert.Single(result.Items);

        Assert.Equal("Karl", user.Name);
        Assert.Equal("karl@marx.de", user.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }


    [Fact]
    public async Task GetUserSummariesCanSearchByName()
    {
        await AuthenticateAsUser(UserRole.Administrator);
        Writer.Seed(db =>
        {
            db.Users.AddRange(
                new User
                {
                    Name = new UserName("Jefke 3"),
                    Email = new UserEmail("Jefke3@jef.jef")
                },
                new User
                {
                    Name = new UserName("Karl"),
                    Email = new UserEmail("Karl@Marx.de")
                });
        });

        var response = await Client.GetAsync("/users?search=Karl");

        var result = await response.ReadJsonAs<GetUserSummariesResponse>(HttpStatusCode.OK);

        var user = Assert.Single(result.Items);

        Assert.Equal("Karl", user.Name);
        Assert.Equal("karl@marx.de", user.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }


    [Fact]
    public async Task GetUserSummariesAppliesPagingAfterSearch()
    {
        await AuthenticateAsUser(UserRole.Administrator);
        Writer.Seed(db =>
        {
            db.Users.AddRange(
            new User
            {
                Name = new UserName("Karl"),
                Email = new UserEmail("karl@marx.de")
            },
            new User
            {
                Name = new UserName("Friedrich"),
                Email = new UserEmail("friedrich@engels.de")
            },
            new User
            {
                Name = new UserName("Jane"),
                Email = new UserEmail("jane.austen@email.com")
            },
            new User
            {
                Name = new UserName("George"),
                Email = new UserEmail("george.orwell@email.com")
            });
        });

        var response = await Client.GetAsync("/users?search=email&page=2&pageSize=1");

        var result = await response.ReadJsonAs<GetUserSummariesResponse>(HttpStatusCode.OK);

        var user = Assert.Single(result.Items);

        Assert.Equal("george.orwell@email.com", user.Email);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
    }

    [Fact]
    public async Task GetUserSummariesSearchForNoResuls()
    {
        await AuthenticateAsUser(UserRole.Administrator);
        Writer.Seed(db =>
            {
                db.Users.AddRange(
                new User
                {
                    Name = new UserName("Karl"),
                    Email = new UserEmail("karl@marx.de")
                },
                new User
                {
                    Name = new UserName("Friedrich"),
                    Email = new UserEmail("friedrich@engels.de")
                },
                new User
                {
                    Name = new UserName("Jane"),
                    Email = new UserEmail("jane.austen@email.com")
                },
                new User
                {
                    Name = new UserName("George"),
                    Email = new UserEmail("george.orwell@email.com")
                });
            });
        var response = await Client.GetAsync("/users?search=Commits&page=2&pageSize=1");

        var result = await response.ReadJsonAs<GetUserSummariesResponse>(HttpStatusCode.OK);

        Assert.Empty(result.Items);

        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);
    }
}

