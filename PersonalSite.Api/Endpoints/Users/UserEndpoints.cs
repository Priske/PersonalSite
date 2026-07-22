using System.Security.Claims;
using PersonalSite.Api.Application.Users.CreateUsers;
using PersonalSite.Api.Application.Users.DeleteUser;
using PersonalSite.Api.Application.Users.GetUserDetails;
using PersonalSite.Api.Application.Users.GetUserSummeries;
using PersonalSite.Api.Application.Users.UpdateUsers;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.Users;

public static class UserEndpoints
{

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", GetUserList)
            .RequireAuthorization();
        app.MapGet("/users/{id:int}", GetUserDetails)
            .RequireAuthorization();

        app.MapPost("/users", CreateUser);

        app.MapPut("users", UpdateUser);
        app.MapPut("/users/{id:int}", UpdateUser)
            .RequireAuthorization();

        app.MapDelete("/users/{id:int}", DeleteUser)
            .RequireAuthorization();

        return app;

    }
    public static async Task<IResult> GetUserList(
    [AsParameters] GetUserSummariesRequest request,
    ClaimsPrincipal principal,
    GetUserSummeriesQueryHandler handler)
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

    public static async Task<IResult> CreateUser(
            CreateUserRequest request,
            CreateUserCommandHandler handler)
    {
        try
        {
            var response = await handler.Execute(request);
            return Results.Created($"/users/{response.Id}", response);
        }
        catch (UserEmailAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }

    }

    public static async Task<IResult> UpdateUser(
        int id,
        ClaimsPrincipal principal,
        UpdateUserRequest request,
        UpdateUserCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var updated = await handler.Execute(actor, id, request);

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
        catch (UserEmailAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    public static async Task<IResult> GetUserDetails(
        int id,
        ClaimsPrincipal principal,
        GetUserDetailsQueryHandler query)
    {
        try
        {
            var actor = principal.ToActor();
            var member = await query.Execute(actor, id);

            if (member is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(member);
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

    public static async Task<IResult> DeleteUser(
       int id,
       ClaimsPrincipal principal,
       DeleteUserCommandHandler handler)
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
