
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Storage.Users;

public class EfUserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User> AddAsync(User user)
    {
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            return false;
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(UserEmail email, int? userIdToIgnore = null)
    {
        var all = await dbContext.Users.ToListAsync();

        return all.Any(u =>
            u.Email == email &&
            (!userIdToIgnore.HasValue || u.Id != userIdToIgnore));
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == id);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existingUser = await dbContext.Users.FindAsync(user.Id);

        if (existingUser is null)
        {
            return false;
        }

        existingUser.Email = user.Email;
        existingUser.Name = user.Name;

        await dbContext.SaveChangesAsync();

        return true;
    }
}