using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.Tags.DeleteTag;

public sealed class DeleteTagCommandHandler(
    ITagRepository tagRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id)
    {
        Permissions.EnsureCanManage(actor);

        return await tagRepository.DeleteAsync(id);
    }
}