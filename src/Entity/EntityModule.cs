using Avolutions.BAF.Core.Entity.Abstractions;
using Avolutions.BAF.Core.Entity.Services;
using Avolutions.BAF.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Entity;

public class EntityModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));
    }
}