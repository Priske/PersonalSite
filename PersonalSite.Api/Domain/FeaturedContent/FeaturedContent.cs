using PersonalSite.Api.Domain.Files;
using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Domain.FeaturedContent;

public sealed class FeaturedContent : SiteContent
{
    public static readonly FileRules VideoFileRules = new(
        AllowedExtensions: [".mp4", ".webm"],
        AllowedContentTypes: ["video/mp4", "video/webm"],
        MaxFileSize: 100 * 1024 * 1024);

    public static readonly FileRules ImageFileRules = new(
        AllowedExtensions: [".jpg", ".jpeg", ".png", ".webp"],
        AllowedContentTypes:
        [
            "image/jpeg",
            "image/png",
            "image/webp"
        ],
        MaxFileSize: 10 * 1024 * 1024);

    public static readonly FileRules DocumentFileRules = new(
        AllowedExtensions: [".pdf"],
        AllowedContentTypes: ["application/pdf"],
        MaxFileSize: 10 * 1024 * 1024);

    public int Id { get; private set; }

    public required FeaturedContentTitle Title { get; set; }
    public required FeaturedContentDescription Description { get; set; }

    private readonly List<FeaturedContentFile> _files = [];

    public IReadOnlyCollection<FeaturedContentFile> Files =>
        _files;

    private readonly List<Tag> _tags = [];

    public IReadOnlyCollection<Tag> Tags =>
        _tags;

    public void AddFile(StoredFile file)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (_files.Any(
                attachment =>
                    attachment.File.StorageKey == file.StorageKey))
        {
            throw new InvalidOperationException(
                "This file is already attached.");
        }

        var rules = ResolveFileRules(file.ContentType);

        FileValidator.Validate(
            file.OriginalFileName,
            file.ContentType,
            file.SizeInBytes,
            rules);

        _files.Add(
            new FeaturedContentFile(
                file,
                sortOrder: _files.Count));
    }

    public void RemoveFile(int storedFileId)
    {
        var attachment = _files.FirstOrDefault(
            item => item.StoredFileId == storedFileId);

        if (attachment is not null)
        {
            _files.Remove(attachment);
        }
    }

    private static FileRules ResolveFileRules(
        string contentType)
    {
        if (VideoFileRules.AllowedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            return VideoFileRules;
        }

        if (ImageFileRules.AllowedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            return ImageFileRules;
        }

        if (DocumentFileRules.AllowedContentTypes.Contains(
                contentType,
                StringComparer.OrdinalIgnoreCase))
        {
            return DocumentFileRules;
        }

        throw new ArgumentException(
            $"Content type '{contentType}' is not supported.");
    }
}