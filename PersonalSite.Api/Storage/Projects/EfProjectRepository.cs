
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Storage.Projects;

public class EfProjectRepository(AppDbContext dbContext) : IProjectRepository
{
    public async Task<Project> AddAsync(Project project)
    {
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await dbContext.Projects.FindAsync(id);

        if (project is null)
        {
            return false;
        }
        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await dbContext.Projects
           .AsNoTracking()
           .SingleOrDefaultAsync(project => project.Id == id);
    }

    public async Task<bool> ProjectExistsAsync(Project project)
    {
        var exists = await dbContext.Projects.FindAsync(project.Id);
        if (exists is null) return false;
        return true;
    }

    public async Task<bool> UpdateAsync(Project project)
    {
        var existingProject = await dbContext.Projects.FindAsync(project.Id);

        if (existingProject is null)
        {
            return false;
        }

        existingProject.Description = project.Description;
        existingProject.IsFeatured = project.IsFeatured;
        existingProject.Title = project.Title;
        existingProject.LiveUrl = project.LiveUrl;
        existingProject.RepositoryUrl = project.RepositoryUrl;
        existingProject.DisplayOrder = project.DisplayOrder;

        await dbContext.SaveChangesAsync();

        return true;
    }
}