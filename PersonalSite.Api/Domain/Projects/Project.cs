using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Tags;

namespace PersonalSite.Api.Domain.Projects;

public sealed class Project : SiteContent
{
    public int Id { get; init; }

    public required ProjectTitle Title { get; set; }

    public required ProjectDescription Description { get; set; }

    public required Url RepositoryUrl { get; set; }

    public Url? LiveUrl { get; set; }

    public bool IsFeatured { get; set; }

    public int DisplayOrder { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];

}