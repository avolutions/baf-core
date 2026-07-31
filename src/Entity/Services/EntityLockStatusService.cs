using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Exceptions;
using Avolutions.Baf.Core.Entity.Models;
using Avolutions.Baf.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Services;

public class EntityLockStatusService<TEntity> : IEntityLockStatusService<TEntity>
    where TEntity : class, ILockable
{
    private readonly IDbContextFactory<BafDbContext> _contextFactory;

    public EntityLockStatusService(IDbContextFactory<BafDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<TEntity> SetLockStatusAsync(Guid id, EntityLockLevel level, string? message = null,
        CancellationToken cancellationToken = default)
    {
        await using var db = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await db.Set<TEntity>().FindAsync([id], cancellationToken);
        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }
        
        entity.LockStatus.Level = level;
        entity.LockStatus.Message = message;
        
        await db.SaveChangesAsync(cancellationToken);
        
        return entity;
    }

    public async Task<TEntity> ClearLockStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await SetLockStatusAsync(id, EntityLockLevel.None, null, cancellationToken);
    }
}