using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Domain.Projects;

public sealed class Project : SiteContent
{
    public int Id { get; init; }

    public required ProjectTitle Title { get; set; }
    public required ProjectDescription Description { get; set; }
    public required Url RepositoryUrl { get; set; }

    public Url? LiveUrl { get; set; }

    public bool IsFeatured { get; set; }

    public int DisplayOrder { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];

    public static Project Create(
        Actor actor,
        ProjectTitle title,
        ProjectDescription description,
        int displayOrder,
        Url repositoryUrl,
        Url? liveUrl,
        bool isFeatured,
        ICollection<Tag> tags)
    {
        var now = DateTimeOffset.UtcNow;

        return new Project
        {
            Title = title,
            Description = description,
            DisplayOrder = displayOrder,
            RepositoryUrl = repositoryUrl,
            LiveUrl = liveUrl,
            IsFeatured = isFeatured,
            Tags = tags,

            Source = actor.Role == UserRole.Administrator
                ? ContentSource.Official
                : ContentSource.Demo,

            Created = new Change(actor.UserId, now),
            Edited = new Change(actor.UserId, now)
        };
    }
}