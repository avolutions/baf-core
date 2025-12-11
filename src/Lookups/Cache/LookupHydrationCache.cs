using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Avolutions.Baf.Core.Caching.Abstractions;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Avolutions.Baf.Core.Lookups.Attributes;
using Avolutions.Baf.Core.Lookups.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Lookups.Cache;

public class LookupHydrationCache : ILookupHydrationCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private ConcurrentDictionary<Type, LookupPropertyMetadata[]> _cache = new();

    public LookupHydrationCache(IServiceScopeFactory scopeFactory, IServiceProvider serviceProvider)
    {
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
    }

    public LookupPropertyMetadata[]? GetMetadata(Type entityType)
    {
        return _cache.TryGetValue(entityType, out var metadata) ? metadata : null;
    }

    public Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();

        var newCache = new ConcurrentDictionary<Type, LookupPropertyMetadata[]>();

        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            var metadata = BuildMetadata(clrType);

            if (metadata.Length > 0)
            {
                newCache[clrType] = metadata;
            }
        }

        _cache = newCache;
        return Task.CompletedTask;
    }

    private LookupPropertyMetadata[] BuildMetadata(Type entityType)
    {
        var result = new List<LookupPropertyMetadata>();
        var properties = entityType.GetProperties();

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<LookupAttribute>() == null)
            {
                continue;
            }

            var idPropertyName = $"{property.Name}Id";
            var idProperty = entityType.GetProperty(idPropertyName);

            if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            {
                continue;
            }

            var cacheType = typeof(ILookupCache<>).MakeGenericType(property.PropertyType);
            var cache = _serviceProvider.GetService(cacheType);

            if (cache == null)
            {
                continue;
            }

            var baseCacheType = typeof(ICache<>).MakeGenericType(property.PropertyType);
            var getByIdMethod = baseCacheType.GetMethod("GetByIdAsync", [typeof(Guid), typeof(CancellationToken)]);

            if (getByIdMethod == null)
            {
                continue;
            }

            result.Add(new LookupPropertyMetadata
            {
                GetId = BuildGetIdDelegate(entityType, idProperty),
                SetLookup = BuildSetLookupDelegate(entityType, property),
                GetFromCache = BuildCacheResolver(cache, getByIdMethod)
            });
        }

        return result.ToArray();
    }

    /// <summary>
    /// Builds a compiled delegate to get the Id property value from an entity.
    /// For an Article with QuantityUnitId, this compiles to:
    /// <code>(object entity) => ((Article)entity).QuantityUnitId</code>
    /// </summary>
    private static Func<object, Guid> BuildGetIdDelegate(Type entityType, PropertyInfo idProperty)
    {
        var parameter = Expression.Parameter(typeof(object), "entity");
        var cast = Expression.Convert(parameter, entityType);
        var propertyAccess = Expression.Property(cast, idProperty);
        var lambda = Expression.Lambda<Func<object, Guid>>(propertyAccess, parameter);
        return lambda.Compile();
    }

    /// <summary>
    /// Builds a compiled delegate to set the lookup property on an entity.
    /// For an Article with QuantityUnit, this compiles to:
    /// <code>(object entity, object? value) => ((Article)entity).QuantityUnit = (QuantityUnit)value</code>
    /// </summary>
    private static Action<object, object?> BuildSetLookupDelegate(Type entityType, PropertyInfo lookupProperty)
    {
        var entityParam = Expression.Parameter(typeof(object), "entity");
        var valueParam = Expression.Parameter(typeof(object), "value");
        var castEntity = Expression.Convert(entityParam, entityType);
        var castValue = Expression.Convert(valueParam, lookupProperty.PropertyType);
        var propertyAccess = Expression.Property(castEntity, lookupProperty);
        var assign = Expression.Assign(propertyAccess, castValue);
        var lambda = Expression.Lambda<Action<object, object?>>(assign, entityParam, valueParam);
        return lambda.Compile();
    }

    /// <summary>
    /// Builds a delegate to resolve a lookup from cache by Id.
    /// This compiles to:
    /// <code>(Guid id) => cache.GetByIdAsync(id, CancellationToken.None).Result</code>
    /// </summary>
    private static Func<Guid, object?> BuildCacheResolver(object cache, MethodInfo getByIdMethod)
    {
        return id =>
        {
            var task = (Task)getByIdMethod.Invoke(cache, [id, CancellationToken.None])!;
            task.GetAwaiter().GetResult();
            return task.GetType().GetProperty("Result")!.GetValue(task);
        };
    }
}