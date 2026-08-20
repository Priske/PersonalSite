using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PersonalSite.Api.Application.FeaturedContent.AddFeaturedContentFile;
using PersonalSite.Api.Application.FeaturedContent.CreateFeaturedContent;
using PersonalSite.Api.Application.FeaturedContent.GetFeaturedContent;
using PersonalSite.Api.Application.FeaturedContent.GetFeaturedContentDetails;
using PersonalSite.Api.Application.FeaturedContent.RemoveFeaturedContentFile;
using PersonalSite.Api.Application.FeaturedContent.UpdateFeaturedContent;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.FeaturedContent;

public static class FeaturedContentEndpoints
{
    public static IEndpointRouteBuilder MapFeaturedContentEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/featured-content", GetFeaturedContent);

        app.MapGet("/featured-content/{id:int}", GetFeaturedContentDetails)
            .RequireAuthorization();

        app.MapPost("/featured-content", CreateFeaturedContent)
            .RequireAuthorization();

        app.MapPut("/featured-content/{id:int}", UpdateFeaturedContent)
            .RequireAuthorization();

        app.MapPost(
                "/featured-content/{id:int}/files",
                AddFile)
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithMetadata(
                new RequestSizeLimitAttribute(
                    110 * 1024 * 1024));

        app.MapDelete(
                "/featured-content/{id:int}/files/{fileId:int}",
                RemoveFile)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> GetFeaturedContentDetails(
        int id,
        ClaimsPrincipal principal,
        GetFeaturedContentDetailsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.Execute(
                id,
                principal.ToActor(),
                cancellationToken);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    private static async Task<IResult> GetFeaturedContent(
        GetFeaturedContentQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.Execute(cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateFeaturedContent(
        CreateFeaturedContentRequest request,
        ClaimsPrincipal principal,
        CreateFeaturedContentCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await handler.Execute(
                request,
                principal.ToActor(),
                cancellationToken);

            return Results.Created(
                $"/featured-content/{created.Id}",
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

    private static async Task<IResult> AddFile(
        int id,
        IFormFile file,
        ClaimsPrincipal principal,
        AddFeaturedContentFileCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = file.OpenReadStream();

            var created = await handler.Execute(
                principal.ToActor(),
                id,
                stream,
                file.FileName,
                file.ContentType,
                file.Length,
                cancellationToken);

            return created is null
                ? Results.NotFound()
                : Results.Created(
                    $"/featured-content/{id}/files/{created.Id}",
                    created);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(
                new { error = exception.Message });
        }
    }

    private static async Task<IResult> UpdateFeaturedContent(
        int id,
        UpdateFeaturedContentRequest request,
        ClaimsPrincipal principal,
        UpdateFeaturedContentCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await handler.Execute(
                principal.ToActor(),
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

    private static async Task<IResult> RemoveFile(
        int id,
        int fileId,
        ClaimsPrincipal principal,
        RemoveFeaturedContentFileCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var removed = await handler.Execute(
                principal.ToActor(),
                id,
                fileId,
                cancellationToken);

            return removed
                ? Results.NoContent()
                : Results.NotFound();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }
}
