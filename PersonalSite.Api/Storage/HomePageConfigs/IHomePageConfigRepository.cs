using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Storage.HomePageConfigs;

public interface IHomePageConfigRepository
{

    Task<HomePageConfig?> GetAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);

}
