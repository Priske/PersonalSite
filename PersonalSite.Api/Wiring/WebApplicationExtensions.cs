
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Wiring;

public static class WebApplicationExtensions
{
    public static WebApplication UseBookTracker(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Member>>();

            dbContext.Database.Migrate();
            /*
            if (app.Configuration.GetValue<bool>("SeedDatabase"))
            {

                DatabaseSeeder.SeedBooks(dbContext, 500);
                DatabaseSeeder.SeedAdministrator(
                    dbContext,
                    app.Configuration,
                    passwordHasher);
                DatabaseSeeder.SeedMembers(dbContext, 200);
            }
            */
        }

        app.UseAuthentication();
        app.UseAuthorization();

        // app.MapBookEndpoints();
        //app.MapMemberEndpoints();
        //app.MapAuthEndpoints();

        return app;
    }

}