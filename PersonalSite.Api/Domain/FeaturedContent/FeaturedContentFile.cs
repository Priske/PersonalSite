using PersonalSite.Api.Domain.Files;

namespace PersonalSite.Api.Domain.FeaturedContent;

public sealed class FeaturedContentFile
{
    private FeaturedContentFile()
    {
    }

    internal FeaturedContentFile(
        StoredFile file,
        int sortOrder)
    {
        File = file;
        SortOrder = sortOrder;
    }

    public int FeaturedContentId { get; private set; }

    public int StoredFileId { get; private set; }
    public StoredFile File { get; private set; } = null!;

    public int SortOrder { get; private set; }
    public string? Caption { get; private set; }

    public void SetCaption(string? caption)
    {
        Caption = string.IsNullOrWhiteSpace(caption)
            ? null
            : caption.Trim();
    }
}