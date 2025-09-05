using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Identity.Abstractions;
using Avolutions.Baf.Core.Identity.Caching;
using Avolutions.Baf.Core.Identity.Models;
using Avolutions.Baf.Core.Identity.Services;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Identity;

public class IdentityModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<IEntityService<User>, UserService>();
        
        services.AddSingleton<IUserDisplayService, DefaultUserDisplayService>();
        services.AddSingleton<IUserCache, UserCache>();
        
        services.AddHostedService<UserCacheInitializer>();
    }
}