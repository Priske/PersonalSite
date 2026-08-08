using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage.Projects;

namespace PersonalSite.Api.Application.Projects.DeleteProject;

public class DeleteProjectCommandHandler(
    IProjectRepository projectRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (project is null)
        {
            return false;
        }

        ProjectPermissions.EnsureCanManage(
            actor,
            project);

        await projectRepository.DeleteAsync(
            project,
            cancellationToken);

        return true;
    }
}