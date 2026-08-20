using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Application.Tags.GetTagSummaries;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.FeaturedContent.GetFeaturedContentDetails;

public sealed class GetFeaturedContentDetailsQueryHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task<GetFeaturedContentDetailsResponse?> Execute(
        int id,
        Actor actor,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new ForbiddenOperationException(
                "Only administrators can manage featured content.");
        }

        var content = await dbContext.FeaturedContents
            .AsNoTracking()
            .Include(item => item.Files)
            .ThenInclude(attachment => attachment.File)
            .Include(item => item.Tags)
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken);

        return content is null
            ? null
            : new GetFeaturedContentDetailsResponse
            {
                Id = content.Id,
                Title = content.Title.Value,
                Description = content.Description.Value,
                Files = content.Files
                    .OrderBy(attachment => attachment.StoredFileId)
                    .Select(attachment => new FeaturedContentFileDetails(
                        attachment.File.Id,
                        attachment.File.OriginalFileName,
                        attachment.File.ContentType,
                        attachment.File.SizeInBytes))
                    .ToList(),
                Tags = content.Tags
                    .OrderBy(tag => tag.Name.Value)
                    .Select(tag => new TagSummary
                    {
                        Id = tag.Id,
                        Name = tag.Name.Value,
                        Source = tag.Source.ToString(),
                        CreatedByUserId = tag.Created.UserId,
                        CreatedAt = tag.Created.At,
                        LastEditedByUserId = tag.Edited.UserId,
                        LastEditedAt = tag.Edited.At
                    })
                    .ToList()
            };
    }
}
