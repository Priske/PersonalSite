using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.Tags.CreateTag;

public sealed class CreateTagCommandHandler(
    ITagRepository tagRepository) : IHandler
{
    public async Task<CreateTagResponse> Execute(
        Actor actor,
        CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        var name = new TagName(request.Name);

        if (await tagRepository.TagExistsAsync(name))
        {
            throw new DomainException(
                "A tag with this name already exists.");
        }

        var tag = Tag.Create(
            actor,
            name);

        await tagRepository.AddAsync(
            tag,
            cancellationToken);

        return new CreateTagResponse
        {
            Id = tag.Id,
            Name = tag.Name.Value,

            Source = tag.Source.ToString(),

            CreatedByUserId = tag.Created.UserId,
            CreatedAt = tag.Created.At,

            LastEditedByUserId = tag.Edited.UserId,
            LastEditedAt = tag.Edited.At
        };
    }
}