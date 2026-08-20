using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Domain.Skills;

public sealed class SkillGroup : SiteContent
{
    public int Id { get; init; }

    public required SkillGroupName Name { get; set; }

    public int DisplayOrder { get; set; }

    public ICollection<Skill> Skills { get; } = [];

    public static SkillGroup Create(
        Actor actor,
        SkillGroupName name,
        int displayOrder)
    {
        var now = DateTimeOffset.UtcNow;

        return new SkillGroup
        {
            Name = name,
            DisplayOrder = displayOrder,
            Source = actor.Role == UserRole.Administrator
                ? ContentSource.Official
                : ContentSource.Demo,
            Created = new Change(actor.UserId, now),
            Edited = new Change(actor.UserId, now)
        };
    }
}
