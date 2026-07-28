using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Endpoints.Auth;
using PersonalSite.Api.Endpoints.HomePageConfigs;
using PersonalSite.Api.Endpoints.Projects;
using PersonalSite.Api.Endpoints.Skills;
using PersonalSite.Api.Endpoints.Users;
using PersonalSite.Api.Seeding;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Wiring;

public static class WebApplicationExtensions
{
    public static WebApplication UsePersonalSite(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseStartup");
        logger.LogInformation("Applying database migrations");

        dbContext.Database.Migrate();

        logger.LogInformation("Database migrations completed");

        if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("SeedDatabase"))
        {
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
            var userFuzzr = scope.ServiceProvider.GetRequiredService<UserFuzzr>();
            var projectFuzzr = scope.ServiceProvider.GetRequiredService<ProjectFuzzr>();
            var tagFuzzr = scope.ServiceProvider.GetRequiredService<TagFuzzr>();

            DatabaseSeeder.SeedUsers(dbContext, userFuzzr, count: 50);
            DatabaseSeeder.SeedAdministrator(dbContext, app.Configuration, passwordHasher);
            DatabaseSeeder.SeedSkills(dbContext);
            DatabaseSeeder.SeedProjects(dbContext, projectFuzzr, DatabaseSeeder.SeedTags(dbContext, tagFuzzr));
            DatabaseSeeder.SeedHomePageConfig(dbContext);
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapUserEndpoints();
        app.MapAuthEndpoints();
        app.MapSkillEndpoints();
        app.MapSkillGroupEndpoints();
        app.MapProjectEndpoints();
        app.MapHomePageEndpoints();

        return app;
    }
}