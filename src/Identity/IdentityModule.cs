using Avolutions.Baf.Core.Identity.Services;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Identity;

public class IdentityModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<UserService>();
    }
}