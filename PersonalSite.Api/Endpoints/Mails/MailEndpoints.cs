
using PersonalSite.Api.Application.Mails.SendContactMails.cs;
using PersonalSite.Api.Domain.Exceptions;

namespace PersonalSite.Api.Endpoints.Mails;

public static class MailEndpoints
{
    public static IEndpointRouteBuilder MapMailEndpoints(
        this IEndpointRouteBuilder app)
    {

        app.MapPost("/contact", SendContactMail);
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
            return Results.BadRequest(new
            {
                error = exception.Message
            });
        }
    }


}