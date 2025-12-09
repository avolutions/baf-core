namespace Avolutions.Baf.Core.Caching.Abstractions;

public interface ICache
{
    Task RefreshAsync(CancellationToken cancellationToken = default);
}

public interface ICache<T> : ICache
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}