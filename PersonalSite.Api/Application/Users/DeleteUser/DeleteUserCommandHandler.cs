using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Users.DeleteUser;

public class DeleteUserCommandHandler(IUserRepository userRepositoryRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id)
    {
        UserPermissions.EnsureCanManage(actor, id);
        return await userRepositoryRepository.DeleteAsync(id);
    }
}