using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Interceptors;
using Avolutions.Baf.Core.Entity.Services;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Avolutions.Baf.Core.Entity;

public class EntityModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));
        services.TryAddSingleton<TrackableSaveChangesInterceptor>();
        services.AddSingleton(typeof(IEntityRouteProvider<>), typeof(EntityRouteProvider<>));
    }
}