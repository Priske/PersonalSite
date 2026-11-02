using PersonalSite.Api.Application.Redirects;

namespace PersonalSite.Api.Endpoints.Redirects;

public static class RedirectEndpoints
{
    public static IEndpointRouteBuilder MapRedirectEndpoints(this IEndpointRouteBuilder app)
    {
        var administration = app.MapGroup("/redirect-links").RequireAuthorization(policy => policy.RequireRole("Administrator"));
        administration.MapGet("", async (HttpContext context, RedirectLinkHandler handler, CancellationToken cancellationToken) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            return Results.Ok(await handler.ListAsync(cancellationToken));
        });
        administration.MapPut("/{slug}", async (string slug, SaveRedirectLinkRequest request, HttpContext context,
            RedirectLinkHandler handler, CancellationToken cancellationToken) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            var errors = RedirectLinkHandler.Validate(slug, request);
            if (errors.Count > 0) return Results.ValidationProblem(errors);
            return Results.Ok(await handler.SaveAsync(slug, request, cancellationToken));
        });

        app.MapGet("/r/{slug}", async (string slug, HttpContext context, RedirectLinkHandler handler, CancellationToken cancellationToken) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            var target = await handler.ResolveAsync(slug, context.Request.Headers.Referer.ToString(), cancellationToken);
            return target is null ? Results.NotFound() : Results.Redirect(target, permanent: false);
        }).AllowAnonymous();

        app.MapGet("/analytics/redirect-links", async ([AsParameters] RedirectAnalyticsRequest request,
            HttpContext context, RedirectLinkHandler handler, CancellationToken cancellationToken) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            if (request.From > request.To)
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["to"] = ["End date must not precede start date."] });
            return Results.Ok(await handler.AnalyticsAsync(request, cancellationToken));
        }).RequireAuthorization(policy => policy.RequireRole("Administrator"));
        return app;
    }
}
