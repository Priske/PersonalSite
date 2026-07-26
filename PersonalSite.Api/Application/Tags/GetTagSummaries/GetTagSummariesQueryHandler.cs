using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Tags.GetTagSummaries;

public class GetTagSummariesQueryHandler(
    AppDbContext dbContext) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<GetTagSummariesResponse> Execute(
        GetTagSummariesRequest request)
    {
        var page = Math.Max(1, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(
            request.PageSize ?? DefaultPageSize,
            MinPage,
            MaxPageSize);

        var query = dbContext.Tags.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search.Trim()}%";

            query = query.Where(tag =>
                EF.Functions.Like(
                    (string)tag.Name,
                    search));
        }

        var totalItems = await query.CountAsync();

        var tags = await query
            .OrderBy(tag => tag.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(tag =>
                new TagSummary
                {
                    Id = tag.Id,
                    Name = tag.Name
                })
            .ToListAsync();

        return new GetTagSummariesResponse
        {
            Items = tags,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(
                totalItems / (double)pageSize)
        };
    }
}