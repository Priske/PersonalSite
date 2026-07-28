using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Domain.HomePageConfigs;

public static class HomePageConfigPermissions
{
    public static void EnsureCanManage(
        Actor actor)
    {
        if (actor.Role == UserRole.Administrator)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot manage Home page configurations.");
    }

    public static void EnsureCanViewDirectory(Actor actor)
    {
        if (actor.Role == UserRole.Administrator)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot view Home page configurations.");
    }
}
