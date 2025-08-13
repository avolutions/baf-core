using Avolutions.BAF.Core.Identity.Hooks;
using Avolutions.BAF.Core.Modules.Abstractions;
using Avolutions.BAF.Core.Persistence.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Identity;

public class IdentityModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddTransient<IModelCreatingHook, IdentityJoinEntitiesHook>();
    }
}