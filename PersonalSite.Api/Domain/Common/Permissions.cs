using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Domain.Common;

public static class Permissions
{
    public static void EnsureCanManage(
        Actor actor)
    {
        if (actor.Role == UserRole.Administrator)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot manage projects.");
    }

    public static void EnsureCanViewDirectory(Actor actor)
    {
        if (actor.Role == UserRole.Administrator)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot view projects.");
    }
}
