using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Storage.Skills;

namespace PersonalSite.Api.Application.Skills.UpdateSkill;

public sealed class UpdateSkillOrderHandler(
    ISkillRepository skillRepository) : IHandler
{
    public async Task Execute(
        Actor actor,
        int groupId,
        UpdateSkillOrderRequest request,
        CancellationToken cancellationToken)
    {
        Permissions.EnsureCanManage(actor);

        await skillRepository.UpdateOrderAsync(
            groupId,
            request.SkillIds,
            cancellationToken);
    }
}