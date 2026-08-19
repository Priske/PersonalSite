using System.Security.Claims;
using PersonalSite.Api.Application.Files;
using PersonalSite.Api.Storage.Files;

namespace PersonalSite.Api.Endpoints.Files;

public static class FileEndpoints
{
    public static IEndpointRouteBuilder MapFileEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/files/cv", UploadCv)
            .RequireAuthorization()
            .DisableAntiforgery();
        app.MapGet("/files/cv", GetCv);
        return app;
    }

    private static async Task<IResult> UploadCv(
    IFormFile file,
    ClaimsPrincipal principal,
    UploadCvCommandHandler handler,
    CancellationToken cancellationToken)
    {
        try
        {
            var actor = principal.ToActor();

            await using var stream = file.OpenReadStream();

            await handler.ExecuteAsync(
                actor,
                stream,
                file.FileName,
                file.ContentType,
                file.Length,
                cancellationToken);

            return Results.NoContent();
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    private static async Task<IResult> GetCv(
     IFileStorage fileStorage,
     CancellationToken cancellationToken)
    {
        var stream = await fileStorage.OpenReadAsync(
            "cv.pdf",
            cancellationToken);

        if (stream is null)
        {
            return Results.NotFound();
        }

        return Results.File(
            stream,
            contentType: "application/pdf",
            fileDownloadName: "Ben-Eeckman-CV.pdf");
    }
}