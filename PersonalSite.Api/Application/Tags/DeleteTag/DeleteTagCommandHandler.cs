using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.Tags.DeleteTag;

public sealed class DeleteTagCommandHandler(
    ITagRepository tagRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
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

        var isInUse =
            await tagRepository.IsInUseAsync(
                id,
                cancellationToken);

        if (isInUse)
        {
            throw new TagInUseException(
                "This tag cannot be deleted because it is used by site content.");
        }

        await tagRepository.DeleteAsync(
            tag,
            cancellationToken);

        return true;
    }
}
