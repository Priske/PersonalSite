using System.Net;
using PersonalSite.Api.Application.Analytics.GetDeleteUserActivity;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Analytics;

public sealed class DeleteUserAnalyticsTests : IntegrationTest
{
    [Fact]
    public async Task DeleteUserAnalyticsReturnsSuccessfulAndFailedAttempts()
    {
        var actorId = await AuthenticateAsUser(
            UserRole.Administrator);

        var targetUser = new User
        {
            Name = new UserName("Delete Target"),
            Email = new UserEmail("delete-target@example.com"),
            Role = UserRole.User
        };

        Writer.Seed(db => db.Users.Add(targetUser));

        var successfulDelete = await Client.DeleteAsync(
            $"/users/{targetUser.Id}");

        await successfulDelete.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);

        const int missingUserId = 999999;

        var failedDelete = await Client.DeleteAsync(
            $"/users/{missingUserId}");

        await failedDelete.ShouldHaveStatusCode(
            HttpStatusCode.BadRequest);

        var response = await Client.GetAsync(
            "/analytics/delete-users?page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<DeleteUserAnalyticsResponse>(
            HttpStatusCode.OK);

        Assert.Equal(2, analytics.Summary.TotalAttempts);
        Assert.Equal(1, analytics.Summary.SuccessfulDeletes);
        Assert.Equal(1, analytics.Summary.FailedDeletes);

        Assert.Contains(
            analytics.Items,
            item =>
                item.UserId == actorId &&
                item.TargetUserId == targetUser.Id &&
                item.Successful &&
                item.FailureReason is null);

        Assert.Contains(
            analytics.Items,
            item =>
                item.UserId == actorId &&
                item.TargetUserId == missingUserId &&
                !item.Successful &&
                item.FailureReason == "unknown_delete_user");
    }

    [Fact]
    public async Task DeleteUserAnalyticsCanFilterFailedAttempts()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        await Client.DeleteAsync(
            "/users/999999");

        var response = await Client.GetAsync(
            "/analytics/delete-users?successful=false&page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<DeleteUserAnalyticsResponse>(
            HttpStatusCode.OK);

        var item = Assert.Single(analytics.Items);

        Assert.False(item.Successful);
        Assert.Equal(999999, item.TargetUserId);
        Assert.Equal("unknown_delete_user", item.FailureReason);
    }

    [Fact]
    public async Task DeleteUserAnalyticsCanSearchTargetUserId()
    {
        await AuthenticateAsUser(
            UserRole.Administrator);

        await Client.DeleteAsync(
            "/users/123456");

        await Client.DeleteAsync(
            "/users/987654");

        var response = await Client.GetAsync(
            "/analytics/delete-users?search=123456&page=1&pageSize=20");

        var analytics = await response.ReadJsonAs<DeleteUserAnalyticsResponse>(
            HttpStatusCode.OK);

        var item = Assert.Single(analytics.Items);

        Assert.Equal(123456, item.TargetUserId);
    }
}
