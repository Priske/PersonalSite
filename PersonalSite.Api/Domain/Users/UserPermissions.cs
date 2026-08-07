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
            "This actor cannot view the user directory.");
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

    public static void EnsureCanDelete(Actor actor, User targetUser)
    {
        if (actor.Role == UserRole.Administrator)
        {
            return;
        }

        if (targetUser.Role == UserRole.FakeUser)
        {
            return;
        }

        throw new ForbiddenOperationException("Forbidden to Delete this user");
    }
}