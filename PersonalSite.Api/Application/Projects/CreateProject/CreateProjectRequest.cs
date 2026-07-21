namespace PersonalSite.Api.Application.Projects.CreateProject;

public class CreateProjectRequest
{
    public required string Title { get; set; }

    public required string Discription { get; set; }

    public required string RepositoryUrl { get; set; }

    public string? LiveUrl { get; set; }
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
}
