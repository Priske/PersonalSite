using PersonalSite.Api.Application.Mails.SendContactMails;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Wiring;

namespace PersonalSite.Api.Endpoints.Mails;

public static class MailEndpoints
{
    public static IEndpointRouteBuilder MapMailEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/contact", SendContactMail)
            .RequireRateLimiting(
                RateLimitPolicies.ContactMail)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static async Task<IResult> SendContactMail(
        SendContactMailRequest request,
        SendContactMailCommandHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            await handler.Execute(
                request,
                cancellationToken);

            return Results.NoContent();
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(
                new
                {
                    error = exception.Message
                });
        }
    }
}