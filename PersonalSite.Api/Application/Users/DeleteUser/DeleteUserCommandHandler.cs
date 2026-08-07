using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions.user;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Users.DeleteUser;

public class DeleteUserCommandHandler(IUserRepository userRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        CancellationToken cancellationToken)
    {
        var targetUser = await userRepository.GetByIdAsync(id, cancellationToken);

        if (targetUser is null)
            throw new UserNotFoundException("Target deleted user could not be found.");


        UserPermissions.EnsureCanDelete(actor, targetUser);

        return await userRepository.DeleteAsync(id, cancellationToken);
    }
}
