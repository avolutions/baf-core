using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Lists;

public class ListsModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddScoped(typeof(IFieldCatalog<>), typeof(FieldCatalog<>));
    }
}