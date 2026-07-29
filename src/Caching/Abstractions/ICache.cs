namespace Avolutions.Baf.Core.Caching.Abstractions;

public interface ICache
{
    Task RefreshAsync(CancellationToken cancellationToken = default);
}

public interface ICache<TKey, T> : ICache
    where TKey : notnull
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
}

public interface ICache<T> : ICache<Guid, T>;