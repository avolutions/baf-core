using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.Search.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Search;

public class SearchModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped<SearchService>();
    }
}