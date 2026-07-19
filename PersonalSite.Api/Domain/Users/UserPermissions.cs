using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Domain.Users;

public static class UserPermissions
{

    public static void EnsureCanViewDirectory(
        Actor actor)
    {
        if (actor.IsAdministrator)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot view the member directory.");
    }

    public static void EnsureCanManage(
        Actor actor,
        int userId)
    {
        if (actor.IsAdministrator)
        {
            return;
        }

        if (actor.UserId == userId)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot manage this user.");
    }
}