namespace PersonalSite.Api.Endpoints.FeaturedContent;

public static class FeaturedContentEndpoints
{
    public static IEndpointRouteBuilder MapFeaturedContentEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/featured-content")
            .RequireAuthorization();

        group.MapPost("/{id:int}/files", AddFeaturedContentFile)
            .DisableAntiforgery();

        group.MapPut("/{id:int}/files/{fileId:int}", UpdateFeaturedContentFile);
        group.MapDelete("/{id:int}/files/{fileId:int}", RemoveFeaturedContentFile);

        return app;
    }

    // Endpoint methods go here.
}