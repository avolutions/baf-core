using System.Collections.Concurrent;
using Avolutions.Baf.Core.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Caching;

public abstract class CacheBase<T> : ICache<T>
{
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private IReadOnlyList<T> _items = [];
    private ConcurrentDictionary<Guid, T> _itemsById = new();

    protected readonly IServiceScopeFactory ScopeFactory;

    protected CacheBase(IServiceScopeFactory scopeFactory)
    {
        ScopeFactory = scopeFactory;
    }

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_items);
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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
            _itemsById = new ConcurrentDictionary<Guid, T>(items.ToDictionary(GetId));
        }
        finally
        {
            _loadLock.Release();
        }
    }

    protected abstract Task<IReadOnlyList<T>> LoadAsync(CancellationToken cancellationToken);

    protected abstract Guid GetId(T item);
}