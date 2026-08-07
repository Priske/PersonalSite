using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Storage.HomePageConfigs;

public interface IHomePageConfigRepository
{

    Task<HomePageConfig?> GetOfficialAsync(CancellationToken cancellationToken);
    Task<HomePageConfig?> GetDemoAsync(int id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);

}
