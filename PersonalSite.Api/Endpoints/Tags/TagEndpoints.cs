using System.Security.Claims;
using PersonalSite.Api.Application.Tags.CreateTag;
using PersonalSite.Api.Application.Tags.DeleteTag;
using PersonalSite.Api.Application.Tags.GetTagDetails;
using PersonalSite.Api.Application.Tags.GetTagSummaries;
using PersonalSite.Api.Application.Tags.UpdateTags;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.Tags;

public static class TagEndpoints
{
    public static IEndpointRouteBuilder MapTagEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/tags", GetTagSummaries)
            .RequireAuthorization();

        app.MapGet("/tags/{id:int}", GetTagDetails)
            .RequireAuthorization();

        app.MapPost("/tags", CreateTag)
            .RequireAuthorization();

        app.MapPut("/tags/{id:int}", UpdateTag)
            .RequireAuthorization();

        app.MapDelete("/tags/{id:int}", DeleteTag)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> GetTagSummaries(
        [AsParameters] GetTagSummariesRequest request,
        GetTagSummariesQueryHandler handler)
    {
        var response = await handler.Execute(request);

        return Results.Ok(response);
    }
    private static async Task<IResult> GetTagDetails(
        int id,
        GetTagDetailsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var tag = await handler.Execute(id, cancellationToken);

        return tag is null
            ? Results.NotFound()
            : Results.Ok(tag);
    }

    private static async Task<IResult> CreateTag(
    ClaimsPrincipal principal,
    CreateTagRequest request,
    CreateTagCommandHandler handler,
    CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            var created = await handler.Execute(
                actor,
                request,
                cancellationToken);

            return Results.Created(
                $"/tags/{created.Id}",
                created);
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(
                new { error = exception.Message });
        }
    }

    private static async Task<IResult> UpdateTag(
        int id,
        UpdateTagRequest request,
        ClaimsPrincipal principal,
        UpdateTagCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            var updated = await handler.Execute(
                actor,
                id,
                request,
                cancellationToken);

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

    private static async Task<IResult> DeleteTag(
    int id,
    ClaimsPrincipal principal,
    DeleteTagCommandHandler handler,
    CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            var deleted = await handler.Execute(
                actor,
                id,
                cancellationToken);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (TagInUseException)
        {
            return Results.Conflict(new
            {
                message = "This tag cannot be deleted because it is used by a project."
            });
        }
    }
}