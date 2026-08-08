using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Storage.Projects;

public interface IProjectRepository
{

    Task<Project> AddAsync(Project project, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Project project, CancellationToken cancellationToken);
    Task DeleteAsync(Project project, CancellationToken cancellationToken);
    Task<bool> ProjectExistsAsync(Project project, CancellationToken cancellationToken);
    Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> UpdateOrderAsync(Actor actor, IReadOnlyList<int> ProjectIds, CancellationToken cancellationToken);
}
