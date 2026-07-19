

using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Users.UpdateUsers;

public class UpdateMemberCommandHandler(IUserRepository userRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        UpdateUserRequest request)
    {

        UserPermissions.EnsureCanManage(actor, id);
        var mail = new UserEmail(request.Email);
        if (await userRepository.EmailExistsAsync(mail, id))
        {
            throw new UserEmailAlreadyExistsException();
        }
        var user =
                new User
                {
                    Id = id,
                    Name = new UserName(request.Name),
                    Email = new UserEmail(request.Email)
                };

        return await userRepository.UpdateAsync(user);
    }
}
