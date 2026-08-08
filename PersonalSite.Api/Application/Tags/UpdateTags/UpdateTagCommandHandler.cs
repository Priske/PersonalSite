using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.Tags.UpdateTags;

public sealed class UpdateTagCommandHandler(
    ITagRepository tagRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        var tag = await tagRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (tag is null)
        {
            return false;
        }

        TagPermissions.EnsureCanManage(
            actor,
            tag);

        var name = new TagName(request.Name);

        if (await tagRepository.TagExistsAsync(
            name,
            id))
        {
            throw new DomainException(
                "A tag with this name already exists.");
        }

        tag.Name = name;

        tag.Edited = new Change(
            actor.UserId,
            DateTimeOffset.UtcNow);

        return await tagRepository.UpdateAsync(
            tag,
            cancellationToken);
    }
}