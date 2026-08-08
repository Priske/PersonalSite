using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Users.GetUserSummeries;



public class GetUserSummeriesQueryHandler(AppDbContext dbContext) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<GetUserSummariesResponse> Execute(
        Actor actor,
        GetUserSummariesRequest request)
    {
        var page = Math.Max(1, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(request.PageSize ?? DefaultPageSize, MinPage, MaxPageSize);

        var query = dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search.Trim()}%";

            query = query.Where(user =>
                EF.Functions.ILike((string)user.Name, search) ||
                EF.Functions.ILike((string)user.Email, search));
        }

        switch (actor.Role)
        {
            case UserRole.Administrator:
                break;
            case UserRole.User:
                query = query.Where(
                    user => user.Role == UserRole.FakeUser);
                break;
            default:
                throw new UnauthorizedAccessException();
        }

        var totalItems =
            await query.CountAsync();

        var users =
            await query
                .OrderBy(user => user.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(user => new UserSummary
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                })
                .ToListAsync();

        return
            new GetUserSummariesResponse
            {
                Items = users,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
    }
}