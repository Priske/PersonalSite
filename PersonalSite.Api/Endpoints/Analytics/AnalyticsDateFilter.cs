using System.Globalization;
using PersonalSite.Api.Application.Analytics;

namespace PersonalSite.Api.Endpoints.Analytics;

public sealed class AnalyticsDateFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var query = context.HttpContext.Request.Query;
        var errors = new Dictionary<string, string[]>();
        if (query.TryGetValue("timeZone", out var zone))
        {
            try { AnalyticsCalendar.Resolve(zone.ToString()); }
            catch (Exception exception) when (exception is TimeZoneNotFoundException or InvalidTimeZoneException or ArgumentException)
            {
                errors["timeZone"] = ["Choose a valid named time zone."];
            }
        }
        if (DateTimeOffset.TryParse(query["from"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var from) &&
            DateTimeOffset.TryParse(query["to"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var to) && from > to)
            errors["to"] = ["End date must not precede start date."];

        return errors.Count > 0 ? Results.ValidationProblem(errors) : await next(context);
    }
}
