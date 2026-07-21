using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Storage.Projects;

public interface IProjectRepository
{

    Task<Project> AddAsync(Project project);
    Task<bool> UpdateAsync(Project project);
    Task<bool> DeleteAsync(int id);
    Task<bool> ProjectExistsAsync(Project project);
    Task<Project?> GetByIdAsync(int id);
}
