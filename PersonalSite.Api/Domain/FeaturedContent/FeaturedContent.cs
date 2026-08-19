using PersonalSite.Api.Domain.Files;
using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Domain.FeaturedContent;

public sealed class FeaturedContent : SiteContent
{
    private FeaturedContent() { }

    public int Id { get; private set; }

    public required FeaturedContentTitle Title { get; set; }
    public required FeaturedContentDescription Description { get; set; }

    public ICollection<FeaturedContentFile> Files { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
}
