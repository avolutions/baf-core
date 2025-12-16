using Avolutions.Baf.Core.Caching.Abstractions;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Avolutions.Baf.Core.Lookups.Cache;
using Avolutions.Baf.Core.Lookups.Interceptors;
using Avolutions.Baf.Core.Lookups.Services;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Lookups;

public class LookupsModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ILookupHydrationCache, LookupHydrationCache>();
        services.AddSingleton<ICache>(sp => sp.GetRequiredService<ILookupHydrationCache>());
        services.AddSingleton<ILookupHydrator, LookupHydrator>();
        services.AddSingleton<LookupHydrationInterceptor>();
        services.AddSingleton<LookupSaveChangesInterceptor>();
    }
}