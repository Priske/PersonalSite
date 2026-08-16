using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage.Files;

namespace PersonalSite.Api.Application.Files;

public sealed class UploadCvCommandHandler(
    IFileStorage fileStorage) : IHandler
{
    private static readonly FileUploadRules Rules = new(
        AllowedExtensions:
        [
            ".pdf"
        ],
        AllowedContentTypes:
        [
            "application/pdf"
        ],
        MaxFileSize: 5 * 1024 * 1024);

    public async Task ExecuteAsync(
    Actor actor,
    Stream content,
    string fileName,
    string contentType,
    long fileSize,
    CancellationToken cancellationToken)
    {
        if (actor.Role != UserRole.Administrator)
        {
            throw new UnauthorizedAccessException(
                "Only administrators can upload the CV.");
        }

        FileValidator.Validate(
            fileName,
            contentType,
            fileSize,
            Rules);

        await fileStorage.UploadAsync(
            "cv.pdf",
            content,
            "application/pdf",
            overwrite: true,
            cancellationToken);
    }
}