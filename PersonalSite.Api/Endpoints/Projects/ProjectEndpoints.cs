using System.Security.Claims;
using PersonalSite.Api.Application.Projects.CreateProject;
using PersonalSite.Api.Application.Projects.DeleteProject;
using PersonalSite.Api.Application.Projects.GetProjectDetails;
using PersonalSite.Api.Application.Projects.GetProjectSummeries;
using PersonalSite.Api.Application.Projects.UpdateProjects;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.Projects;

public static class ProjectEndpoints
{

    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/projects", GetProjectList);

        app.MapGet("/projects/{id:int}", GetProjectDetails);

        app.MapPost("/projects", CreateProject).RequireAuthorization();

        app.MapPut("/projects/{id:int}", UpdateProject).RequireAuthorization();

        app.MapDelete("/projects/{id:int}", DeleteProject).RequireAuthorization();

        app.MapPut("/projects/order", UpdateProjectOrder).RequireAuthorization();

        return app;

    }
    public static async Task<IResult> GetProjectList(
    [AsParameters] GetProjectSummariesRequest request,
    ClaimsPrincipal principal,
    GetProjectSummeriesQueryHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var response = await handler.Execute(actor, request);

            return Results.Ok(response);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    public static async Task<IResult> CreateProject(
    CreateProjectRequest request,
    ClaimsPrincipal principal,
    CreateProjectCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();

            var created =
                await handler.Execute(request, actor);

            return Results.Created(
                $"/projects/{created.Id}",
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

    public static async Task<IResult> UpdateProject(
        int id,
        ClaimsPrincipal principal,
        UpdateProjectRequest request,
        UpdateProjectCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();
            var updated = await handler.Execute(actor, id, request, cancellationToken);

            if (!updated)
            {
                return Results.NotFound();
            }
            return Results.NoContent();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    private static async Task<IResult> UpdateProjectOrder(
     ClaimsPrincipal principal,
     UpdateProjectsOrderRequest request,
     UpdateProjectsGroupOrderHandler handler,
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

    public static async Task<IResult> GetProjectDetails(
        int id,
        GetProjectDetailsQueryHandler query)
    {
        var project = await query.Execute(id);

        return project is null
            ? Results.NotFound()
            : Results.Ok(project);
    }

    public static async Task<IResult> DeleteProject(
       int id,
       ClaimsPrincipal principal,
       DeleteProjectCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var deleted = await handler.Execute(actor, id);

            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }

    }
}
