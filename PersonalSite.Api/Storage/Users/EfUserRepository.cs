
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Exceptions.user;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Seeding;

namespace PersonalSite.Api.Storage.Users;

public class EfUserRepository(AppDbContext dbContext, UserFuzzr userFuzzr) : IUserRepository
{
    public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
    public async Task<bool> AddFakeUsersAsync(CancellationToken cancellationToken)
    {
        var fakeUserCount = dbContext.Users.Where(u => u.Role == UserRole.FakeUser).Count();
        if (fakeUserCount > 20)
        {
            throw new UserOutOfRangeException("Too many Fake users still in the system");
        }
        var users = await userFuzzr.ManyAsync(30);
        dbContext.Users.AddRange(users);

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync([id], cancellationToken);

        if (user is null)
        {
            return false;
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EmailExistsAsync(UserEmail email, CancellationToken cancellationToken, int? userIdToIgnore = null)
    {
        var all = await dbContext.Users.ToListAsync(cancellationToken);

        return all.Any(u =>
            u.Email == email &&
            (!userIdToIgnore.HasValue || u.Id != userIdToIgnore));
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var existingUser = await dbContext.Users.FindAsync([user.Id], cancellationToken);

        if (existingUser is null)
        {
            return false;
        }

        existingUser.Email = user.Email;
        existingUser.Name = user.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}