using System.Linq.Expressions;
using System.Reflection;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Attributes;
using Avolutions.Baf.Core.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Entity.Cache;

public class EntityNavigationCache<TEntity> : IEntityNavigationCache<TEntity>
    where TEntity : class, IEntity, INavigable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly Expression<Func<TEntity, object>> _orderByExpression;

    private IReadOnlyList<Guid> _orderedIds = Array.Empty<Guid>();
    private Dictionary<Guid, int> _idToIndex = new();

    public EntityNavigationCache(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _orderByExpression = BuildOrderByExpression();
    }

    private static Expression<Func<TEntity, object>> BuildOrderByExpression()
    {
        var property = typeof(TEntity)
            .GetProperties()
            .FirstOrDefault(p => p.GetCustomAttribute<EntityNavigationKeyAttribute>() != null);

        if (property == null)
        {
            throw new InvalidOperationException($"Entity {typeof(TEntity).Name} must have a property with [NavigationKey] attribute.");
        }

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var propertyAccess = Expression.Property(parameter, property);
        var converted = Expression.Convert(propertyAccess, typeof(object));

        return Expression.Lambda<Func<TEntity, object>>(converted, parameter);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DbContext>();

            var ordered = await context.Set<TEntity>()
                .AsNoTracking()
                .OrderBy(_orderByExpression)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            var index = new Dictionary<Guid, int>(ordered.Count);
            for (var i = 0; i < ordered.Count; i++)
            {
                index[ordered[i]] = i;
            }

            _orderedIds = ordered;
            _idToIndex = index;
        }
        finally
        {
            _lock.Release();
        }
    }

    public Task<EntityNavigationResult> GetNavigationAsync(Guid currentId, CancellationToken cancellationToken = default)
    {
        var ids = _orderedIds;

        if (ids.Count == 0)
        {
            return Task.FromResult(new EntityNavigationResult(null, null, null, null, null, 0));
        }

        if (!_idToIndex.TryGetValue(currentId, out var currentIndex))
        {
            return Task.FromResult(new EntityNavigationResult(ids[0], null, null, ids[^1], null, ids.Count));
        }

        return Task.FromResult(new EntityNavigationResult(
            ids[0],
            currentIndex > 0 ? ids[currentIndex - 1] : null,
            currentIndex < ids.Count - 1 ? ids[currentIndex + 1] : null,
            ids[^1],
            currentIndex,
            ids.Count));
    }
}