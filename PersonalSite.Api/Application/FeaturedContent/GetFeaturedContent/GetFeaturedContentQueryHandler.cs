using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.FeaturedContent.GetFeaturedContent;

public sealed class GetFeaturedContentQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetFeaturedContentResponse> Execute(
        CancellationToken cancellationToken)
    {
        var content = await dbContext.FeaturedContents
            .AsNoTracking()
            .Where(item => item.Source == ContentSource.Official)
            .Include(item => item.Files)
            .ThenInclude(attachment => attachment.File)
            .Include(item => item.Tags)
            .OrderBy(item => item.Id)
            .ToListAsync(cancellationToken);

        return new GetFeaturedContentResponse(
            content.Select(item => new FeaturedContentResponse(
                    item.Id,
                    item.Title.Value,
                    item.Description.Value,
                    item.Files
                        .OrderBy(attachment => attachment.StoredFileId)
                        .Select(attachment => new FeaturedContentFileResponse(
                            attachment.File.Id,
                            attachment.File.OriginalFileName,
                            attachment.File.ContentType,
                            attachment.File.SizeInBytes))
                        .ToList(),
                    item.Tags
                        .OrderBy(tag => tag.Name.Value)
                        .Select(tag => tag.Name.Value)
                        .ToList()))
                .ToList());
    }
}
