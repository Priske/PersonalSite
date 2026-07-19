using System.Security.Claims;
using PersonalSite.Api.Application.Users.GetUserSummeries;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.Users;

public static class UserEndpoints
{

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", GetUserList)
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

}
