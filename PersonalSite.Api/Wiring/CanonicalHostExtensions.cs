namespace PersonalSite.Api.Wiring;

public static class CanonicalHostExtensions
{
    public static WebApplication UseCanonicalHostRedirect(
        this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Host.Host.Equals(
                "www.beneeckman.be",
                StringComparison.OrdinalIgnoreCase))
            {
                var destination =
                    $"https://beneeckman.be{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";

                context.Response.Redirect(
                    destination,
                    permanent: true);

                return;
            }

            await next();
        });

        return app;
    }
}