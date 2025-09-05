using Avolutions.Baf.Core.Audit.Abstractions;
using Avolutions.Baf.Core.Audit.Models;
using Avolutions.Baf.Core.Entity.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Audit.Services
{
    public sealed class AuditLogService<TEntity> : IAuditLogService<TEntity> 
        where TEntity : class, IEntity
    {
        private readonly DbContext _context;
        private readonly string _entityName;

        public AuditLogService(DbContext context)
        {
            _context = context;
            _entityName = typeof(TEntity).Name;
        }
        
        public async Task<IReadOnlyList<AuditLog>> GetAllAsync(
            Guid entityId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<AuditLog>()
                .AsNoTracking()
                .Where(a => a.Entity == _entityName && a.EntityId == entityId)
                .OrderByDescending(a => a.CreatedAt)
                .ThenByDescending(a => a.Id)
                .ToListAsync(cancellationToken);
        }
    }
}