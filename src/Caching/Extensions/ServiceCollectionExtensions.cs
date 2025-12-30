using Avolutions.Baf.Core.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Caching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class, ICache
        where TImplementation : class, TInterface
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton<TInterface>(sp => sp.GetRequiredService<TImplementation>());
        services.AddSingleton<ICache>(sp => sp.GetRequiredService<TImplementation>());
        return services;
    }
}