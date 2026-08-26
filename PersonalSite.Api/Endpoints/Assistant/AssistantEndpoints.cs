using System.Security.Claims;
using PersonalSite.Api.Application.Assistant;
using PersonalSite.Api.Domain.Exceptions.Assistant;
using PersonalSite.Api.Wiring;

namespace PersonalSite.Api.Endpoints.Assistant;

public static class AssistantEndpoints
{
    public static IEndpointRouteBuilder MapAssistantEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/assistant/ask", AskQuestion)
            .RequireRateLimiting(RateLimitPolicies.Assistant)
            .Produces<AskQuestionResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

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

        if (
            httpContext.User.Identity
                ?.IsAuthenticated == true)
        {
            var idClaim =
                httpContext.User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (
                int.TryParse(
                    idClaim?.Value,
                    out var id))
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
        catch (
            InvalidAssistantQuestionException exception)
        {
            return Results.BadRequest(
                new
                {
                    error = exception.Message
                });
        }
        catch (
            AssistantUnavailableException exception)
        {
            var logger =
                loggerFactory.CreateLogger("PersonalSite.Api.Endpoints.Assistant");

            logger.LogWarning(
                exception,
                "The assistant request failed: {Reason}",
                exception.Message);

            return Results.Problem(
                title:
                    "Assistant temporarily unavailable",
                detail:
                    "The portfolio assistant is currently unavailable. Please try again later.",
                statusCode:
                    StatusCodes.Status503ServiceUnavailable);
        }
    }
}