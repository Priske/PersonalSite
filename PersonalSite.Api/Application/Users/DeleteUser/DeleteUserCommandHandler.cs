using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions.user;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Users.DeleteUser;

public class DeleteUserCommandHandler(
    IUserRepository userRepository,
    ActivityTracker activityTracker) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        CancellationToken cancellationToken)
    {
        var targetUser = await userRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (targetUser is null)
        {
            await activityTracker.TrackAsync(
                ActivityType.DeleteUser,
                actor.UserId,
                metadata =>
                {
                    metadata.Add(
                        "reason",
                        new StringMetadataValue("unknown_delete_user"));

                    metadata.Add(
                        "attempted_delete_user",
                        new IntegerMetadataValue(id));
                },
                cancellationToken);

            throw new UserNotFoundException(
                "Target deleted user could not be found.");
        }

        UserPermissions.EnsureCanDelete(
            actor,
            targetUser);

        var deleted = await userRepository.DeleteAsync(
            id,
            cancellationToken);

        if (deleted)
        {
            await activityTracker.TrackAsync(
                ActivityType.DeleteUser,
                actor.UserId,
                metadata =>
                {
                    metadata.Add(
                        "deleted_user",
                        new IntegerMetadataValue(id));
                },
                cancellationToken);
        }

        return deleted;
    }
}