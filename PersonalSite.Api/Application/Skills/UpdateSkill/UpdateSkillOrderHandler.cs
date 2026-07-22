using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Application.Skills.UpdateSkill;

public sealed class UpdateSkillOrderHandler(
    AppDbContext dbContext) : IHandler
{
    public async Task Execute(
        int groupId,
        UpdateSkillOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (request.SkillIds.Count == 0)
        {
            return;
        }

        if (request.SkillIds.Distinct().Count() !=
            request.SkillIds.Count)
        {
            throw new ArgumentException(
                "A skill cannot appear more than once.");
        }

        var skills = await dbContext.Skills
            .Where(skill =>
                skill.SkillGroupId == groupId)
            .ToListAsync(cancellationToken);

        if (skills.Count != request.SkillIds.Count)
        {
            throw new ArgumentException(
                "The supplied skill list does not contain every skill in the group.");
        }

        var skillsById = skills.ToDictionary(
            skill => skill.Id);

        if (request.SkillIds.Any(
                skillId =>
                    !skillsById.ContainsKey(skillId)))
        {
            throw new ArgumentException(
                "One or more skills do not belong to this group.");
        }

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(
                cancellationToken);


        for (var index = 0;
             index < request.SkillIds.Count;
             index++)
        {
            var skill =
                skillsById[request.SkillIds[index]];

            skill.DisplayOrder =
                10_000 + index;
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);

        for (var index = 0;
             index < request.SkillIds.Count;
             index++)
        {
            var skill =
                skillsById[request.SkillIds[index]];

            skill.DisplayOrder = index + 1;
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);

        await transaction.CommitAsync(
            cancellationToken);
    }
}