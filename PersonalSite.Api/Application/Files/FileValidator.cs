namespace PersonalSite.Api.Application.Files;

public static class FileValidator
{
    public static void Validate(
        string fileName,
        string contentType,
        long fileSize,
        FileUploadRules rules)
    {
        if (fileSize <= 0)
        {
            throw new ArgumentException("File is empty.");
        }

        if (fileSize > rules.MaxFileSize)
        {
            throw new ArgumentException(
                $"File cannot be larger than {rules.MaxFileSize} bytes.");
        }

        var extension = Path.GetExtension(fileName);

        if (!rules.AllowedExtensions.Contains(
                extension,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"File extension '{extension}' is not allowed.");
        }

        if (!rules.AllowedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"Content type '{contentType}' is not allowed.");
        }
    }
}