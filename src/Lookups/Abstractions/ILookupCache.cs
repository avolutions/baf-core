using Avolutions.Baf.Core.Caching.Abstractions;
using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Lookups.Abstractions;

public interface ILookupCache<T> : ICache<T>
    where T : class, ILookup, IEntity
{
    Task<T> GetDefaultAsync(CancellationToken cancellationToken = default);
    Task<Guid> GetDefaultIdAsync(CancellationToken cancellationToken = default);
}