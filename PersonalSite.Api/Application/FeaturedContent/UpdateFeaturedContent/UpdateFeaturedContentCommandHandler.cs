using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.FeaturedContent;
using PersonalSite.Api.Storage.Files;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.FeaturedContent.UpdateFeaturedContent;

public sealed class UpdateFeaturedContentCommandHandler(
    IFeaturedContentRepository featuredContentRepository,
    ITagRepository tagRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        UpdateFeaturedContentRequest request,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new ForbiddenOperationException(
                "Only administrators can manage featured content.");
        }

        var content = await featuredContentRepository.GetWithFilesAsync(
            id,
            cancellationToken);

        if (content is null || content.Source != ContentSource.Official)
        {
            return false;
        }

        var tagIds = request.TagIds
            .Distinct()
            .ToArray();

        var tags = await tagRepository.GetByIdsAsync(
            tagIds,
            cancellationToken);

        if (tags.Count != tagIds.Length)
        {
            throw new DomainException(
                "One or more selected tags do not exist.");
        }

        content.Title = new FeaturedContentTitle(request.Title);
        content.Description = new FeaturedContentDescription(
            request.Description);

        content.Tags.Clear();

        foreach (var tag in tags)
        {
            content.Tags.Add(tag);
        }

        content.Edited = new Change(
            actor.UserId,
            DateTimeOffset.UtcNow);

        await featuredContentRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}
