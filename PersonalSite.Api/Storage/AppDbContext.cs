using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Projects;
using PersonalSite.Api.Domain.Skills;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Infrastructure.Security.Password;

namespace PersonalSite.Api.Storage;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<CompromisedPasswordHash> CompromisedPasswordHashes => Set<CompromisedPasswordHash>();

    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<SkillGroup> SkillGroups => Set<SkillGroup>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}