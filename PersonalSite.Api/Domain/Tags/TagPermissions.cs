using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Tags;

public static class TagPermissions
{
    public static void EnsureCanManage(
        Actor actor,
        Tag tag)
    {
        if (actor.IsAdministrator)
        {
            return;
        }

        if (
            tag.Source == ContentSource.Demo &&
            tag.Created.UserId == actor.UserId)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot manage this tag.");
    }
}