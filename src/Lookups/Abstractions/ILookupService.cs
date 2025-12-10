using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Lookups.Abstractions;

public interface ILookupService<T> : IEntityService<T>
    where T : class, ILookup, IEntity
{
    Task<T?> GetByIdAsync(Guid id, string language, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(string language, CancellationToken cancellationToken = default);
    Task SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T> GetDefaultAsync(CancellationToken cancellationToken = default);
}