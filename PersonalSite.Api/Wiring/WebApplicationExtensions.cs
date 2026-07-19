
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Endpoints.Auth;
using PersonalSite.Api.Endpoints.Users;
using PersonalSite.Api.Seeding;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Wiring;

public static class WebApplicationExtensions
{
    public static WebApplication UsePersonalSite(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
            var userFuzzr = scope.ServiceProvider.GetRequiredService<UserFuzzr>();

            //dbContext.Database.Migrate();
            dbContext.Database.EnsureCreated();
            if (app.Configuration.GetValue<bool>("SeedDatabase"))
            {
                DatabaseSeeder.SeedUsers(
                dbContext,
                userFuzzr,
                count: 50);

                DatabaseSeeder.SeedAdministrator(
                dbContext,
                app.Configuration,
                passwordHasher);

            }

        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapUserEndpoints();
        app.MapAuthEndpoints();


        return app;
    }

}