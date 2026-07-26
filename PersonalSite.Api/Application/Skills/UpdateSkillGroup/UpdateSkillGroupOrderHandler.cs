using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Storage.Skills;

namespace PersonalSite.Api.Application.Skills.UpdateSkillGroup;

public sealed class UpdateSkillGroupOrderHandler(
    ISkillGroupRepository skillGroupRepository) : IHandler
{
    public async Task Execute(
        Actor actor,
        UpdateSkillGroupOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!actor.IsAdministrator)
        {
            throw new DomainException("Forbidden");
        }

        await skillGroupRepository.UpdateOrderAsync(
            request.SkillGroupIds,
            cancellationToken);
    }
}