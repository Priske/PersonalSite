using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Domain.FeaturedContent;

public sealed class FeaturedContent : SiteContent
{
    private FeaturedContent() { }

    public int Id { get; private set; }

    public required FeaturedContentTitle Title { get; set; }
    public required FeaturedContentDescription Description { get; set; }

    public ICollection<FeaturedContentFile> Files { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];

    public static FeaturedContent Create(
        Actor actor,
        FeaturedContentTitle title,
        FeaturedContentDescription description,
        ICollection<Tag> tags)
    {
        var now = DateTimeOffset.UtcNow;

        return new FeaturedContent
        {
            Title = title,
            Description = description,
            Tags = tags,
            Source = actor.Role == UserRole.Administrator
                ? ContentSource.Official
                : ContentSource.Demo,
            Created = new Change(actor.UserId, now),
            Edited = new Change(actor.UserId, now)
        };
    }
}
