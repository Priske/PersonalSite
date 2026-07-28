using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Projects.GetProjectSummeries;

public class GetProjectSummeriesQueryHandler(AppDbContext dbContext) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<GetProjectSummariesResponse> Execute(
        GetProjectSummariesRequest request)
    {
        var page = Math.Max(MinPage, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(
            request.PageSize ?? DefaultPageSize,
            MinPage,
            MaxPageSize);

        var query = dbContext.Projects
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search.Trim()}%";

            query = query.Where(project =>
                EF.Functions.ILike((string)project.Title, search) ||
                EF.Functions.ILike((string)project.Description, search) ||
                EF.Functions.ILike((string)project.RepositoryUrl, search));
        }

        var totalItems = await query.CountAsync();

        var projectEntities = await query
            .Include(project => project.Tags)
            .OrderBy(project => project.DisplayOrder)
            .ThenBy(project => project.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var projects = projectEntities
            .Select(project => new ProjectSummary
            {
                Id = project.Id,
                Title = project.Title.Value,
                Description = project.Description.Value,
                RepositoryUrl = project.RepositoryUrl.Value,
                LiveUrl = project.LiveUrl?.Value,
                IsFeatured = project.IsFeatured,
                DisplayOrder = project.DisplayOrder,
                Tags = project.Tags
                    .OrderBy(tag => tag.Name.Value)
                    .Select(tag => tag.Name.Value)
                    .ToList()
            })
            .ToList();

        return new GetProjectSummariesResponse
        {
            Items = projects,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(
                totalItems / (double)pageSize)
        };
    }
}