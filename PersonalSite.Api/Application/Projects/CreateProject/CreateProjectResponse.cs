namespace PersonalSite.Api.Application.Projects.CreateProject;

public class CreateProjectResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public required string RepositoryUrl { get; set; }

    public string? LiveUrl { get; set; }
    public bool IsFeatured { get; set; }

    public int DisplayOrder { get; set; }

    public required IReadOnlyList<string> Tags { get; set; }
}


