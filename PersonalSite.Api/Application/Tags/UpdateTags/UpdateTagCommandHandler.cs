using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
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
        UpdateTagRequest request)
    {
        Permissions.EnsureCanManage(actor);

        var name = new TagName(request.Name);

        if (await tagRepository.TagExistsAsync(name, id))
        {
            throw new DomainException(
                "A tag with this name already exists.");
        }

        var tag = new Tag
        {
            Id = id,
            Name = name
        };

        return await tagRepository.UpdateAsync(tag);
    }
}