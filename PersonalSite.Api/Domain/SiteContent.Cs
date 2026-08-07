
namespace PersonalSite.Api.Domain;

public abstract class SiteContent
{
    public ContentSource Source { get; protected set; }

    public int? CreatedByUserId { get; protected set; }

    public int? LastEditedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; protected set; }

    public DateTimeOffset LastEditedAt { get; set; }
}