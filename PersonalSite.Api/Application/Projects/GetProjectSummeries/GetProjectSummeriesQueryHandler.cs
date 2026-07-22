using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Projects.GetProjectSummeries;



public class GetProjectSummeriesQueryHandler(AppDbContext dbContext) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<GetProjectSummariesResponse> Execute(
        Actor actor,
        GetProjectSummariesRequest request)
    {
        ProjectPermissions.EnsureCanViewDirectory(actor);
        var page = Math.Max(1, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(request.PageSize ?? DefaultPageSize, MinPage, MaxPageSize);

        var query = dbContext.Projects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search.Trim()}%";

            query = query.Where(project =>
                EF.Functions.Like((string)project.Title, search) ||
                EF.Functions.Like((string)project.Description, search) ||
                EF.Functions.Like((string)project.RepositoryUrl, search));
        }

        var totalItems = await query.CountAsync();

        var projects = await query
            .OrderBy(project => project.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(project =>
                new ProjectSummary
                {
                    Id = project.Id,
                    Title = project.Title.Value,
                    Discription = project.Description.Value,
                    RepositoryUrl = project.RepositoryUrl.Value,
                    LiveUrl = project.LiveUrl == null ? null : project.LiveUrl.Value,
                    IsFeatured = project.IsFeatured,
                    DisplayOrder = project.DisplayOrder
                })
            .ToListAsync();

        return
            new GetProjectSummariesResponse
            {
                Items = projects,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
    }
}