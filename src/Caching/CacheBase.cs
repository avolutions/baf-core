using System.Collections.Concurrent;
using Avolutions.Baf.Core.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Caching;

public abstract class CacheBase<TKey, T> : ICache<TKey, T>
    where TKey : notnull
{
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<T> _items = [];
    private ConcurrentDictionary<TKey, T> _itemsById;

    protected readonly IServiceScopeFactory ScopeFactory;

    protected CacheBase(IServiceScopeFactory scopeFactory)
    {
        ScopeFactory = scopeFactory;
        _itemsById = new ConcurrentDictionary<TKey, T>(KeyComparer);
    }

    /// <summary>
    /// Override to change key equality, e.g. <see cref="StringComparer.OrdinalIgnoreCase"/>.
    /// </summary>
    protected virtual IEqualityComparer<TKey> KeyComparer => EqualityComparer<TKey>.Default;

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_items);
    }

    public Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        _itemsById.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        await _loadLock.WaitAsync(cancellationToken);
        try
        {
            var items = await LoadAsync(cancellationToken);
            _items = items;
            _itemsById = new ConcurrentDictionary<TKey, T>(
                items.ToDictionary(GetId, KeyComparer),
                KeyComparer);
        }
        finally
        {
            _loadLock.Release();
        }
    }

    protected abstract Task<IReadOnlyList<T>> LoadAsync(CancellationToken cancellationToken);

    protected abstract TKey GetId(T item);
}

public abstract class CacheBase<T> : CacheBase<Guid, T>, ICache<T>
{
    protected CacheBase(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }
}