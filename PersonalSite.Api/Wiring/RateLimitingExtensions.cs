using System.Net;
using System.Threading.RateLimiting;

namespace PersonalSite.Api.Wiring;

public static class RateLimitPolicies
{
    public const string ContactMail = "contact-mail";
}

public static class RateLimitingExtensions
{
    public static WebApplicationBuilder AddPersonalSiteRateLimiting(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (
                context,
                cancellationToken) =>
            {
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        error =
                            "Too many contact messages. Please try again later."
                    },
                    cancellationToken);
            };

            options.AddPolicy<string>(
                RateLimitPolicies.ContactMail,
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey:
                            GetClientAddress(httpContext),
                        factory: _ =>
                            new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 3,
                                Window =
                                    TimeSpan.FromMinutes(10),
                                QueueLimit = 0,
                                QueueProcessingOrder =
                                    QueueProcessingOrder.OldestFirst,
                                AutoReplenishment = true
                            }));
        });

        return builder;
    }

    public static WebApplication UsePersonalSiteRateLimiting(
        this WebApplication app)
    {
        app.UseRateLimiter();

        return app;
    }

    private static string GetClientAddress(
        HttpContext context)
    {
        var forwardedFor =
            context.Request.Headers["X-Forwarded-For"]
                .ToString();

        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var rightmostAddress = forwardedFor
                .Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries)
                .LastOrDefault();

            if (
                rightmostAddress is not null &&
                IPAddress.TryParse(
                    rightmostAddress,
                    out var parsedAddress))
            {
                return parsedAddress.ToString();
            }
        }

        return context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
}