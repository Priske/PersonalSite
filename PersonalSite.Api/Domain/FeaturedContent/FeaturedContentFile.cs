using PersonalSite.Api.Domain.Files;

namespace PersonalSite.Api.Domain.FeaturedContent;

public sealed class FeaturedContentFile
{
    private FeaturedContentFile() { }

    public FeaturedContentFile(StoredFile file)
    {
        File = file;
    }

    public int FeaturedContentId { get; private set; }

    public int StoredFileId { get; private set; }
    public StoredFile File { get; private set; } = null!;
}
