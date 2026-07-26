using System.Security.Claims;
using PersonalSite.Api.Application.Skills.CreateSkillGroup;
using PersonalSite.Api.Application.Skills.DeleteSkillgroup;
using PersonalSite.Api.Application.Skills.GetSkillGroupDetails;
using PersonalSite.Api.Application.Skills.GetSkillGroupSummeries;
using PersonalSite.Api.Application.Skills.UpdateSkill;
using PersonalSite.Api.Application.Skills.UpdateSkillGroup;
using PersonalSite.Api.Domain.Exceptions;


namespace PersonalSite.Api.Endpoints.Skills;

public static class SkillGroupEndpoints
{
    public static IEndpointRouteBuilder MapSkillGroupEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/skill-groups",
            GetSkillGroupSummaries);

        app.MapGet(
            "/skill-groups/{groupId:int}",
            GetSkillGroupDetails);

        app.MapPost(
                "/skill-groups",
                CreateSkillGroup)
            .RequireAuthorization();

        app.MapPut(
                "/skill-groups/{groupId:int}",
                UpdateSkillGroup)
            .RequireAuthorization();
        app.MapPut(
                "/skill-groups/order",
                UpdateSkillGroupOrder)
            .RequireAuthorization();

        app.MapPut(
                "/skill-groups/{groupId:int}/skills/order",
                UpdateSkillOrder)
            .RequireAuthorization();


        app.MapDelete(
                "/skill-groups/{groupId:int}",
                DeleteSkillGroup)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetSkillGroupSummaries(
        GetSkillGroupSummariesQueryHandler handler)
    {
        var response = await handler.Execute();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetSkillGroupDetails(
        int groupId,
        GetSkillGroupDetailsQueryHandler handler)
    {
        try
        {
            var group = await handler.Execute(groupId);

            return group is null
                ? Results.NotFound()
                : Results.Ok(group);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> CreateSkillGroup(
        CreateSkillGroupRequest request,
        ClaimsPrincipal principal,
        CreateSkillGroupCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var created =
                await handler.Execute(actor, request);

            return Results.Created(
                $"/skill-groups/{created.Id}",
                created);
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

    private static async Task<IResult> UpdateSkillGroup(
        int groupId,
        UpdateSkillGroupRequest request,
        ClaimsPrincipal principal,
        UpdateSkillGroupCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var updated = await handler.Execute(
                actor,
                groupId,
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

    private static async Task<IResult> UpdateSkillOrder(
    int groupId,
    UpdateSkillOrderRequest request,
    UpdateSkillOrderHandler handler,
    ClaimsPrincipal principal,
    CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            await handler.Execute(
                actor,
                groupId,
                request,
                cancellationToken);

            return Results.NoContent();
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
    private static async Task<IResult> UpdateSkillGroupOrder(
    UpdateSkillGroupOrderRequest request,
    ClaimsPrincipal principal,
    UpdateSkillGroupOrderHandler handler,
    CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            await handler.Execute(
                actor,
                request,
                cancellationToken);

            return Results.NoContent();
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

    private static async Task<IResult> DeleteSkillGroup(
        int groupId,
        ClaimsPrincipal principal,
        DeleteSkillGroupCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var deleted =
                await handler.Execute(actor, groupId);

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