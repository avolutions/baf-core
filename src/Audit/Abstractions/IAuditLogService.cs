using Avolutions.Baf.Core.Audit.Models;
using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Audit.Abstractions
{
    public interface IAuditLogService<TEntity>
        where TEntity : class, IEntity
    {
        Task<IReadOnlyList<AuditLog>> GetAllAsync(
            Guid entityId,
            CancellationToken cancellationToken = default);
    }
}