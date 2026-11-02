namespace PersonalSite.Api.Domain.Redirects;

public sealed class RedirectLink
{
    public int Id { get; set; }
    public RedirectSlug Slug { get; set; } = null!;
    public RedirectTargetUrl TargetUrl { get; set; } = null!;
    public RedirectCampaign? Campaign { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
