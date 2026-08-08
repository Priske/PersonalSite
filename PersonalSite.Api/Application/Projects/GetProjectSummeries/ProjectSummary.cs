namespace PersonalSite.Api.Application.Projects.GetProjectSummeries;

public class ProjectSummary
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public required string RepositoryUrl { get; set; }

    public string? LiveUrl { get; set; }
    public bool IsFeatured { get; set; }

    public int DisplayOrder { get; set; }

    public required IReadOnlyList<string> Tags { get; set; }

    public required string Source { get; init; }
    public int? CreatedByUserId { get; init; }
    public int? LastEditedByUserId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset LastEditedAt { get; init; }

}