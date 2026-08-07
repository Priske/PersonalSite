
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Domain.Tags;

public sealed class Tag : SiteContent
{
    public int Id { get; set; }
    public TagName Name { get; set; } = null!;
    public ICollection<Project> Projects { get; set; } = [];
}