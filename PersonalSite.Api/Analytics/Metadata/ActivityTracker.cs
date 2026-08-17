using PersonalSite.Api.Storage.Analytics;
namespace PersonalSite.Api.Analytics.Metadata;

public class ActivityTracker(
    IActivityRepository activityRepository)
{
    public async Task TrackAsync(
        ActivityType type,
        int? userId,
        Action<ActivityMetadata>? configureMetadata,
        CancellationToken cancellationToken)
    {
        var activity = new Activity(type, userId);

        if (configureMetadata is not null)
        {
            var metadata = new ActivityMetadata();

            configureMetadata(metadata);

            activity.AddMetadata(metadata);
        }

        await activityRepository.AddAsync(
            activity,
            cancellationToken);
    }
}