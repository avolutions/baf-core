using Avolutions.Baf.Core.Caching.Abstractions;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Entity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityNavigationCache<TEntity>(this IServiceCollection services)
        where TEntity : class, IEntity, INavigable
    {
        var entityType = typeof(TEntity);
        var cacheType = typeof(EntityNavigationCache<>).MakeGenericType(entityType);
        var interfaceType = typeof(IEntityNavigationCache<>).MakeGenericType(entityType);

        services.AddSingleton(interfaceType, cacheType);
        services.AddSingleton(typeof(ICache), sp => sp.GetRequiredService(interfaceType));

        return services;
    }
}