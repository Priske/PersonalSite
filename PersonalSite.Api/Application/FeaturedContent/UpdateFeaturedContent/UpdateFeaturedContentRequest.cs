namespace PersonalSite.Api.Application.FeaturedContent.UpdateFeaturedContent;

public sealed class UpdateFeaturedContentRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int[] TagIds { get; set; } = [];
}
