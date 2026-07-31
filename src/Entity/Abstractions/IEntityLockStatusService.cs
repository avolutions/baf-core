using Avolutions.Baf.Core.Entity.Models;

namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface IEntityLockStatusService<TEntity>
    where TEntity : class, ILockable
{
    Task<TEntity> SetLockStatusAsync(Guid id, EntityLockLevel level, string? message = null, CancellationToken cancellationToken = default);
    Task<TEntity> ClearLockStatusAsync(Guid id, CancellationToken cancellationToken = default);
}