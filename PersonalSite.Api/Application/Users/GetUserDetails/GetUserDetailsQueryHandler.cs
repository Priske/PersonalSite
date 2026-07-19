using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Users.GetUserDetails;


public class GetUserDetailsQueryHandler(AppDbContext dbContext) : IHandler
{
    public async Task<GetUserDetailsResponse?> Execute(
        Actor actor,
        int id)
    {
        UserPermissions.EnsureCanViewDirectory(actor);

        return await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user =>
                new GetUserDetailsResponse
                {
                    Id = user.Id,
                    Name = user.Name.Value,
                    Email = user.Email.Value,
                    Role = user.Role.ToString()
                })
            .FirstOrDefaultAsync();
    }
}