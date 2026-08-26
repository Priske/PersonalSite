using System.Security.Claims;
using PersonalSite.Api.Application.Assistant;
using PersonalSite.Api.Domain.Exceptions.Assistant;

namespace PersonalSite.Api.Endpoints.Assistant;

public static class AssistantEndpoints
{
    public static IEndpointRouteBuilder MapAssistantEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/assistant/ask",
            AskQuestion);

        return app;
    }

    private static async Task<IResult> AskQuestion(
        AskQuestionRequest request,
        AskQuestionCommandHandler handler,
        HttpContext httpContext,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        int? userId = null;

        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var idClaim =
                httpContext.User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (int.TryParse(idClaim?.Value, out var id))
            {
                userId = id;
            }
        }

        try
        {
            var response = await handler.Execute(
                request,
                userId,
                cancellationToken);

            return Results.Ok(response);
        }
        catch (AssistantUnavailableException exception)
        {
            var logger = loggerFactory.CreateLogger(
                "PersonalSite.Api.Endpoints.Assistant");

            logger.LogWarning(
                exception,
                "The assistant request failed: {Reason}",
                exception.Message);

            return Results.Problem(
                title: "Assistant temporarily unavailable",
                detail: "Please try again later.",
                statusCode:
                    StatusCodes.Status503ServiceUnavailable);
        }
    }
}