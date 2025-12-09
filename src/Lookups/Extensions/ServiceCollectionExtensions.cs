using Avolutions.Baf.Core.Caching.Abstractions;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Avolutions.Baf.Core.Lookups.Cache;
using Avolutions.Baf.Core.Lookups.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Lookups.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLookupCache<T>(this IServiceCollection services)
        where T : class, ILookup, IEntity
    {
        var lookupInterface = typeof(T).GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILookup<>));

        var lookupType = lookupInterface.GetGenericArguments()[0];

        var cacheType = typeof(LookupCache<,>).MakeGenericType(typeof(T), lookupType);
        var lookupInterfaceType = typeof(ILookupCache<>).MakeGenericType(typeof(T));

        services.AddSingleton(lookupInterfaceType, cacheType);
        
        // Also register as ICache so CacheInitializer can find it
        services.AddSingleton(typeof(ICache), sp => sp.GetRequiredService(lookupInterfaceType));

        return services;
    }
    
    public static IServiceCollection AddLookupService<T>(this IServiceCollection services)
        where T : class, ILookup, IEntity
    {
        var lookupInterface = typeof(T).GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILookup<>));

        var lookupType = lookupInterface.GetGenericArguments()[0];

        var serviceType = typeof(LookupService<,>).MakeGenericType(typeof(T), lookupType);
        var entityServiceInterface = typeof(IEntityService<>).MakeGenericType(typeof(T));
        var lookupServiceInterface = typeof(ILookupService<>).MakeGenericType(typeof(T));

        // Register the concrete service
        services.AddScoped(serviceType);
        
        // Register interfaces pointing to the same instance
        services.AddScoped(entityServiceInterface, sp => sp.GetRequiredService(serviceType));
        services.AddScoped(lookupServiceInterface, sp => sp.GetRequiredService(serviceType));

        return services;
    }
    
    public static IServiceCollection AddLookup<T>(this IServiceCollection services)
        where T : class, ILookup, IEntity
    {
        services.AddLookupService<T>();
        services.AddLookupCache<T>();
        return services;
    }
}