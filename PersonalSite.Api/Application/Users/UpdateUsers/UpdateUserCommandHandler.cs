

using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Exceptions.user;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Users.UpdateUsers;

public class UpdateUserCommandHandler(IUserRepository userRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {

        var mail = new UserEmail(request.Email);
        if (await userRepository.EmailExistsAsync(mail, cancellationToken, id))
        {
            throw new UserEmailAlreadyExistsException();
        }
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            throw new InvalidUserRoleException("Invalid user role.");
        }
        var user =
                new User
                {
                    Id = id,
                    Name = new UserName(request.Name),
                    Email = new UserEmail(request.Email),
                    Role = role
                };
        if (role != UserRole.FakeUser)
        {
            UserPermissions.EnsureCanManage(actor, id);
        }
        return await userRepository.UpdateAsync(user, cancellationToken);
    }
}
