namespace PersonalSite.Api.Application.Projects.UpdateProjects;

public class UpdateProjectRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }

    public required string RepositoryUrl { get; set; }

    public string? LiveUrl { get; set; }
    public bool IsFeatured { get; set; }

    public int[] TagIds { get; set; }

}
