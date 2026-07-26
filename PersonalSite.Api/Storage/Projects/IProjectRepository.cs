using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Storage.Projects;

public interface IProjectRepository
{

    Task<Project> AddAsync(Project project);
    Task<bool> UpdateAsync(Project project, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id);
    Task<bool> ProjectExistsAsync(Project project, CancellationToken cancellationToken);
    Task<Project?> GetByIdAsync(int id);
    Task<bool> UpdateOrderAsync(IReadOnlyList<int> ProjectIds, CancellationToken cancellationToken);
}
