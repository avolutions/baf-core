using Avolutions.Baf.Core.Caching.Abstractions;
using Avolutions.Baf.Core.Entity.Models;

namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface IEntityNavigationCache : ICache
{
    Task<EntityNavigationResult> GetNavigationAsync(Guid currentId, CancellationToken cancellationToken = default);
}

public interface IEntityNavigationCache<TEntity> : IEntityNavigationCache
    where TEntity : class, IEntity
{
}