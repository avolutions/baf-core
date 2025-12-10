using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Caching;

public class CachingModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddHostedService<CacheInitializer>();
    }
}