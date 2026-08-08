using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Actors;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Storage.Projects;

public class EfProjectRepository(AppDbContext dbContext) : IProjectRepository
{
    public async Task<Project> AddAsync(Project project, CancellationToken cancellationToken)
    {
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task DeleteAsync(
        Project project,
        CancellationToken cancellationToken)
    {
        dbContext.Projects.Remove(project);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .SingleOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    public async Task<bool> ProjectExistsAsync(
        Project project,
        CancellationToken cancellationToken)
    {
        var existingProject = await dbContext.Projects.FindAsync(
            [project.Id],
            cancellationToken);

        return existingProject is not null;
    }

    public async Task<bool> UpdateAsync(
    Project project,
    CancellationToken cancellationToken)
    {
        var existingProject = await dbContext.Projects
            .Include(current => current.Tags)
            .SingleOrDefaultAsync(
                current => current.Id == project.Id,
                cancellationToken);

        if (existingProject is null)
        {
            return false;
        }

        existingProject.Description = project.Description;
        existingProject.IsFeatured = project.IsFeatured;
        existingProject.Title = project.Title;
        existingProject.LiveUrl = project.LiveUrl;
        existingProject.RepositoryUrl = project.RepositoryUrl;

        existingProject.Tags.Clear();

        foreach (var tag in project.Tags)
        {
            existingProject.Tags.Add(tag);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateOrderAsync(
    Actor actor,
    IReadOnlyList<int> projectIds,
    CancellationToken cancellationToken)
    {
        if (projectIds.Count == 0)
        {
            return false;
        }

        if (projectIds.Distinct().Count() != projectIds.Count)
        {
            throw new DomainException(
                "A project cannot appear more than once.");
        }

        var projects = await dbContext.Projects
            .Where(project => projectIds.Contains(project.Id))
            .ToListAsync(cancellationToken);

        if (projects.Count != projectIds.Count)
        {
            throw new DomainException(
                "One or more projects do not exist.");
        }

        if (!actor.IsAdministrator)
        {
            var containsUnauthorizedProject = projects.Any(project =>
                project.Source != ContentSource.Demo ||
                project.Created.UserId != actor.UserId);

            if (containsUnauthorizedProject)
            {
                throw new ForbiddenOperationException(
                    "You cannot reorder one or more of these projects.");
            }
        }

        var projectsById = projects.ToDictionary(
            project => project.Id);

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        for (var index = 0; index < projectIds.Count; index++)
        {
            projectsById[projectIds[index]].DisplayOrder =
                10_000 + index;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        for (var index = 0; index < projectIds.Count; index++)
        {
            projectsById[projectIds[index]].DisplayOrder =
                index + 1;

            projectsById[projectIds[index]].Edited =
                new Change(
                    actor.UserId,
                    DateTimeOffset.UtcNow);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return true;
    }
}
