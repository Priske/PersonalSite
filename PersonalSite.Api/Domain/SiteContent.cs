
namespace PersonalSite.Api.Domain;

public abstract class SiteContent
{
    public ContentSource Source { get; protected set; }
    public Change Edited { get; set; }
    public Change Created { get; protected set; }

}