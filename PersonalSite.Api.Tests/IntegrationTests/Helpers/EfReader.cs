
using Microsoft.Extensions.DependencyInjection;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Tests.IntegrationTests.Helpers;

public class EfReader(IServiceProvider services)
{
    public T Query<T>(Func<AppDbContext, T> query)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return query(db);
    }
}
