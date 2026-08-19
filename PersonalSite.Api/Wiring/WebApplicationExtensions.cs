using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Endpoints.Analytics;
using PersonalSite.Api.Endpoints.Auth;
using PersonalSite.Api.Endpoints.FeaturedContent;
using PersonalSite.Api.Endpoints.Files;
using PersonalSite.Api.Endpoints.HomePageConfigs;
using PersonalSite.Api.Endpoints.Projects;
using PersonalSite.Api.Endpoints.Skills;
using PersonalSite.Api.Endpoints.Tags;
using PersonalSite.Api.Endpoints.Users;
using PersonalSite.Api.Seeding;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Wiring;

public static class WebApplicationExtensions
{
    public static WebApplication UsePersonalSite(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseStartup");

        var connection = dbContext.Database.GetDbConnection();

        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                SELECT
                    current_database(),
                    current_user,
                    inet_server_addr()::text,
                    inet_server_port(),
                    current_schema();
                """;

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                logger.LogInformation(
                    "Actual PostgreSQL connection: Database={Database}, User={User}, Server={Server}, Port={Port}, Schema={Schema}",
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.IsDBNull(2) ? "unknown" : reader.GetString(2),
                    reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    reader.GetString(4));
            }
        }

        connection.Close();

        var migrations = dbContext.Database.GetMigrations();
        var appliedMigrations = dbContext.Database.GetAppliedMigrations();
        var pendingMigrations = dbContext.Database.GetPendingMigrations();

        logger.LogInformation(
            "Known migrations: {Migrations}",
            string.Join(", ", migrations));

        logger.LogInformation(
            "Applied migrations: {Migrations}",
            string.Join(", ", appliedMigrations));

        logger.LogInformation(
            "Pending migrations: {Migrations}",
            string.Join(", ", pendingMigrations));

        logger.LogInformation("Applying database migrations");

        dbContext.Database.Migrate();

        logger.LogInformation("Database migrations completed");

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<User>>();

        var administrator = DatabaseSeeder.SeedInitialAdministrator(
            dbContext,
            app.Configuration,
            passwordHasher);

        DatabaseSeeder.SeedHomePageConfig(
            dbContext,
            administrator.Id);

        if (
            app.Environment.IsDevelopment() &&
            app.Configuration.GetValue<bool>("SeedDatabase"))
        {
            var userFuzzr = scope.ServiceProvider
                .GetRequiredService<UserFuzzr>();

            var projectFuzzr = scope.ServiceProvider
                .GetRequiredService<ProjectFuzzr>();

            var tagFuzzr = scope.ServiceProvider
                .GetRequiredService<TagFuzzr>();

            DatabaseSeeder.SeedUsers(
                dbContext,
                userFuzzr,
                count: 50);

            DatabaseSeeder.SeedSkills(dbContext);

            var tags = DatabaseSeeder.SeedTags(
                dbContext,
                tagFuzzr,
                administrator.Id);

            DatabaseSeeder.SeedProjects(
                dbContext,
                projectFuzzr,
                tags);
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapFileEndpoints();
        app.MapUserEndpoints();
        app.MapAuthEndpoints();
        app.MapSkillEndpoints();
        app.MapSkillGroupEndpoints();
        app.MapProjectEndpoints();
        app.MapHomePageEndpoints();
        app.MapAnalyticsEndpoints();
        app.MapTagEndpoints();

        return app;
    }
}