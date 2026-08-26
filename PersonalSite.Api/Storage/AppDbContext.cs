
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Domain.Assistant;
using PersonalSite.Api.Domain.FeaturedContent;
using PersonalSite.Api.Domain.Files;
using PersonalSite.Api.Domain.HomePageConfigs;
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
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();

    public DbSet<FeaturedContentFile> FeaturedContentFiles => Set<FeaturedContentFile>();
    public DbSet<AssistantKnowledgeFile> AssistantKnowledgeFiles => Set<AssistantKnowledgeFile>();
    public DbSet<FeaturedContent> FeaturedContents => Set<FeaturedContent>();
    public DbSet<AssistantKnowledge> AssistantKnowledges => Set<AssistantKnowledge>();

    public DbSet<HomePageConfig> HomepageConfigs => Set<HomePageConfig>();


    public DbSet<Activity> Activities => Set<Activity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}