using Avolutions.BAF.Core.Modules.Abstractions;
using Avolutions.BAF.Core.Search.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Search;

public class SearchModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<SearchService>();
    }
}