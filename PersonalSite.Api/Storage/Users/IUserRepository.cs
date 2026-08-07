using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Storage.Users;

public interface IUserRepository
{

    Task<User> AddAsync(User User, CancellationToken cancellationToken);
    Task<bool> AddFakeUsersAsync(CancellationToken cancellationToken);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(UserEmail email, CancellationToken cancellationToken, int? UserIdToIgnore = null);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
