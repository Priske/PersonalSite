namespace PersonalSite.Api.Application.Projects.UpdateProjects;

public sealed record UpdateProjectsOrderRequest(
    IReadOnlyList<int> ProjectIds
);