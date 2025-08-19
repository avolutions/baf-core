using Avolutions.Baf.Core.Menu.Services;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Menu;

public class MenuModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton(typeof(MenuProvider<>));
    }
}