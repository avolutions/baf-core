using Avolutions.Baf.Core.Loading.Services;
using Avolutions.BAF.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Loading;

public class LoadingModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<LoadingService>();
    }
}