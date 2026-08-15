namespace PersonalSite.Api.Analytics;

using PersonalSite.Api.Analytics.Metadata;
public sealed class Activity
{
    private readonly List<ActivityMetadata> _metadata = [];

    private Activity()
    {
    }
    public Activity(
    ActivityType type,
    int? userId = null)
    {
        Type = type;
        UserId = userId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }
    public int? UserId { get; private set; }
    public ActivityType Type { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<ActivityMetadata> Metadata => _metadata;
    public void AddMetadata(ActivityMetadata metadata)
    {
        _metadata.Add(metadata);
    }
}