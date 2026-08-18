using Microsoft.Extensions.DependencyInjection;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Storage;
using PersonalSite.Api.Storage.Analytics;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Storage.Analytics;

public sealed class EFActivityRepositoryTests : IDisposable
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task ActivityMetadataRoundTripsAllSupportedValueTypes()
    {
        using var scope = factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var repository = new EFActivityRepository(dbContext);

        var occurredAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        var details = new ObjectMetadataValue();

        details.Add(
            "text",
            new StringMetadataValue("hello"));

        details.Add(
            "integer",
            new IntegerMetadataValue(42));

        details.Add(
            "decimal",
            new DecimalMetadataValue(12.5m));

        details.Add(
            "boolean",
            new BooleanMetadataValue(true));

        details.Add(
            "dateTime",
            new DateTimeMetadataValue(occurredAt));

        var metadata = new ActivityMetadata();

        metadata.Add(
            "details",
            details);

        var activity = new Activity(
            ActivityType.PageViewed,
            123);

        activity.AddMetadata(metadata);

        await repository.AddAsync(
            activity,
            CancellationToken.None);

        var loadedActivities = await repository.GetAsync(
            ActivityType.PageViewed,
            null,
            null,
            CancellationToken.None);

        var loadedActivity =
            Assert.Single(loadedActivities);

        var loadedMetadata =
            Assert.Single(loadedActivity.Metadata);

        var loadedObject =
            Assert.IsType<ObjectMetadataValue>(
                loadedMetadata.Values["details"]);

        Assert.Equal(
            "hello",
            Assert.IsType<StringMetadataValue>(
                loadedObject.Values["text"]).Value);

        Assert.Equal(
            42,
            Assert.IsType<IntegerMetadataValue>(
                loadedObject.Values["integer"]).Value);

        Assert.Equal(
            12.5m,
            Assert.IsType<DecimalMetadataValue>(
                loadedObject.Values["decimal"]).Value);

        Assert.True(
            Assert.IsType<BooleanMetadataValue>(
                loadedObject.Values["boolean"]).Value);

        var loadedDateTime =
            Assert.IsType<DateTimeMetadataValue>(
                loadedObject.Values["dateTime"]);

        Assert.Equal(
            occurredAt.ToUnixTimeMilliseconds(),
            loadedDateTime.Value.ToUnixTimeMilliseconds());
    }

    public void Dispose()
    {
        factory.Dispose();
    }
}