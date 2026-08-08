using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Domain.Projects;

public static class ProjectPermissions
{
    public static void EnsureCanManage(
        Actor actor,
        Project project)
    {
        if (actor.Role == UserRole.Administrator)
        {
            return;
        }

        if (
            project.Source == ContentSource.Demo &&
            project.Created.UserId == actor.UserId)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot manage this project.");
    }

    public static void EnsureCanCreate(
        Actor actor)
    {
        if (
            actor.Role == UserRole.Administrator ||
            actor.Role == UserRole.User)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot create projects.");
    }

    public static void EnsureCanViewDirectory(
        Actor actor)
    {
        if (
            actor.Role == UserRole.Administrator ||
            actor.Role == UserRole.User)
        {
            return;
        }

        throw new ForbiddenOperationException(
            "This actor cannot view projects.");
    }
}