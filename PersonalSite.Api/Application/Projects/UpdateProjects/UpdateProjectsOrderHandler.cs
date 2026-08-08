using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Storage.Projects;

namespace PersonalSite.Api.Application.Projects.UpdateProjects;

public sealed class UpdateProjectsGroupOrderHandler(
    IProjectRepository projectRepository) : IHandler
{
    public async Task Execute(
        Actor actor,
        UpdateProjectsOrderRequest request,
        CancellationToken cancellationToken)
    {
        await projectRepository.UpdateOrderAsync(
            actor,
            request.ProjectIds,
            cancellationToken);
    }
}