using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Users.GetUserDetails;


public class GetUserDetailsQueryHandler(AppDbContext dbContext) : IHandler
{
    public async Task<GetUserDetailsResponse?> Execute(
     Actor actor,
     int id,
     CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
              .AsNoTracking()
              .Where(user => user.Id == id)
              .Select(user => new GetUserDetailsResponse
              {
                  Id = user.Id,
                  Name = user.Name.Value,
                  Email = user.Email.Value,
                  Role = user.Role.ToString()
              })
              .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            Console.WriteLine($"User {id} was not found.");
            return null;
        }

        if (user.Role == "FakeUser")
        {
            return user;
        }

        UserPermissions.EnsureCanViewDirectory(actor);

        return user;
    }
}