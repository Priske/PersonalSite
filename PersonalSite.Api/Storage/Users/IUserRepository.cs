using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Storage.Users;

public interface IUserRepository
{

    Task<User> AddAsync(User User);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> EmailExistsAsync(UserEmail email, int? UserIdToIgnore = null);
    Task<User?> GetByIdAsync(int id);
}
