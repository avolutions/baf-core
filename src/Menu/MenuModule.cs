using Avolutions.BAF.Core.Menu.Abstractions;
using Avolutions.BAF.Core.Menu.Services;
using Avolutions.BAF.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Menu;

public class MenuModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton(typeof(MenuProvider<>));
    }
}