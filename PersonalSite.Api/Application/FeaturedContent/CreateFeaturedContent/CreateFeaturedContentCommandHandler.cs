using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.FeaturedContent;
using PersonalSite.Api.Storage.Files;
using PersonalSite.Api.Storage.Tags;
using FeaturedContentEntity = PersonalSite.Api.Domain.FeaturedContent.FeaturedContent;

namespace PersonalSite.Api.Application.FeaturedContent.CreateFeaturedContent;

public sealed class CreateFeaturedContentCommandHandler(
    IFeaturedContentRepository featuredContentRepository,
    ITagRepository tagRepository) : IHandler
{
    public async Task<CreateFeaturedContentResponse> Execute(
        CreateFeaturedContentRequest request,
        Actor actor,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new ForbiddenOperationException(
                "Only administrators can create featured content.");
        }

        var tagIds = request.TagIds
            .Distinct()
            .ToList();

        var tags = await tagRepository.GetByIdsAsync(
            tagIds,
            cancellationToken);

        if (tags.Count != tagIds.Count)
        {
            throw new DomainException(
                "One or more selected tags do not exist.");
        }

        var content = FeaturedContentEntity.Create(
            actor,
            new FeaturedContentTitle(request.Title),
            new FeaturedContentDescription(request.Description),
            tags.ToList());

        var saved = await featuredContentRepository.AddAsync(
            content,
            cancellationToken);

        return new CreateFeaturedContentResponse
        {
            Id = saved.Id,
            Title = saved.Title.Value,
            Description = saved.Description.Value,
            Tags = saved.Tags
                .Select(tag => tag.Name.Value)
                .ToList()
        };
    }
}
