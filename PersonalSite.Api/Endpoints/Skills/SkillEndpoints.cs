using System.Security.Claims;
using PersonalSite.Api.Application.Skills.CreateSkill;
using PersonalSite.Api.Application.Skills.DeleteSkill;
using PersonalSite.Api.Application.Skills.GetSkillDetails;
using PersonalSite.Api.Application.Skills.GetSkillSummeries;
using PersonalSite.Api.Application.Skills.UpdateSkill;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.Skills;

public static class SkillEndpoints
{
    public static IEndpointRouteBuilder MapSkillEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/skill-groups/{groupId:int}/skills",
            GetSkillSummaries);

        app.MapGet(
            "/skill-groups/{groupId:int}/skills/{skillId:int}",
            GetSkillDetails);

        app.MapPost(
                "/skill-groups/{groupId:int}/skills",
                CreateSkill)
            .RequireAuthorization();

        app.MapPut(
                "/skill-groups/{groupId:int}/skills/{skillId:int}",
                UpdateSkill)
            .RequireAuthorization();

        app.MapDelete(
                "/skill-groups/{groupId:int}/skills/{skillId:int}",
                DeleteSkill)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetSkillSummaries(
        int groupId,
        ClaimsPrincipal principal,
        GetSkillSummariesQueryHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var skills = await handler.Execute(
                actor,
                groupId);

            return Results.Ok(skills);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> GetSkillDetails(
        int groupId,
        int skillId,
        ClaimsPrincipal principal,
        GetSkillDetailsQueryHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var skill = await handler.Execute(
                actor,
                groupId,
                skillId);

            return skill is null
                ? Results.NotFound()
                : Results.Ok(skill);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> CreateSkill(
        int groupId,
        CreateSkillRequest request,
        ClaimsPrincipal principal,
        CreateSkillCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var created = await handler.Execute(
                actor,
                groupId,
                request);

            return Results.Created(
                $"/skill-groups/{groupId}/skills/{created.Id}",
                created);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (NotFoundException exception)
        {
            return Results.NotFound(
                new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(
                new { error = exception.Message });
        }
    }

    private static async Task<IResult> UpdateSkill(
     int groupId,
     int skillId,
     UpdateSkillRequest request,
     ClaimsPrincipal principal,
     UpdateSkillCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var updated = await handler.Execute(
                actor,
                groupId,
                skillId,
                request);

            return updated
                ? Results.NoContent()
                : Results.NotFound();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(
                new { error = exception.Message });
        }
    }

    private static async Task<IResult> DeleteSkill(
        int groupId,
        int skillId,
        ClaimsPrincipal principal,
        DeleteSkillCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var deleted = await handler.Execute(
                actor,
                groupId,
                skillId);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }
}