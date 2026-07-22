using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Storage.Projects;


namespace PersonalSite.Api.Application.Projects.DeleteProject;

public class DeleteProjectCommandHandler(IProjectRepository projectRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id)
    {
        ProjectPermissions.EnsureCanManage(actor);
        return await projectRepository.DeleteAsync(id);
    }
}