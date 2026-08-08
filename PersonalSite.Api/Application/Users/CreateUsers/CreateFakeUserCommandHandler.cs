using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Users.CreateUsers;

public class CreateFakeUserCommandHandler(
    IUserRepository userRepository) : IHandler
{

    public async Task<bool> Execute(CancellationToken cancellationToken)
    {
        await userRepository.AddFakeUsersAsync(cancellationToken);
        return true;

    }

}
