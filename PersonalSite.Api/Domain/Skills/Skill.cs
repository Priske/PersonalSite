
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Domain.Skills;

public sealed class Skill : SiteContent
{
    public int Id { get; init; }

    public int SkillGroupId { get; init; }
    public SkillGroup SkillGroup { get; private set; } = null!;

    public required SkillName SkillName { get; set; }

    public int DisplayOrder { get; set; }

    public static Skill Create(
        Actor actor,
        int skillGroupId,
        SkillName name,
        int displayOrder)
    {
        var now = DateTimeOffset.UtcNow;

        return new Skill
        {
            SkillGroupId = skillGroupId,
            SkillName = name,
            DisplayOrder = displayOrder,
            Source = actor.Role == UserRole.Administrator
                ? ContentSource.Official
                : ContentSource.Demo,
            Created = new Change(actor.UserId, now),
            Edited = new Change(actor.UserId, now)
        };
    }
}
