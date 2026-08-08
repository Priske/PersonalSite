using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Auth.GetCurrentUser;

public class GetCurrentUserQueryHandler(IUserRepository userRepository) : IHandler
{
    public async Task<CurrentUserResponse?> Execute(int id, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new CurrentUserResponse
        {
            Id = user.Id,
            Name = user.Name.Value,
            Email = user.Email.Value,
            Role = user.Role.ToString()
        };
    }
}