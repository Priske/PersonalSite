using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Domain.Tags;

public sealed class Tag : SiteContent
{
    public int Id { get; set; }

    public TagName Name { get; set; } = null!;

    public ICollection<Project> Projects { get; set; } = [];

    public static Tag Create(
        Actor actor,
        TagName name)
    {
        var now = DateTimeOffset.UtcNow;

        return new Tag
        {
            Name = name,

            Source = actor.IsAdministrator
                ? ContentSource.Official
                : ContentSource.Demo,

            Created = new Change(
                actor.UserId,
                now),

            Edited = new Change(
                actor.UserId,
                now)
        };
    }
}