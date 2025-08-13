using Avolutions.BAF.Core.Entities.Abstractions;
using Avolutions.BAF.Core.Entities.Services;
using Avolutions.BAF.Core.Modules.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Entities;

public class EntityModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));
    }
}