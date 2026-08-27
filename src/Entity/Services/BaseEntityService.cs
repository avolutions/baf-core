using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Exceptions;
using Avolutions.Baf.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Services;

public class BaseEntityService<TEntity> : IEntityService<TEntity>
    where TEntity : class, IEntity
{
    protected readonly IDbContextFactory<BafDbContext> ContextFactory;

    public BaseEntityService(IDbContextFactory<BafDbContext> contextFactory)
    {
        ContextFactory = contextFactory;
    }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        return await context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        return await context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public virtual async Task<TEntity?> GetByExternalIdAsync(
        string externalId,
        CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        return await context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ExternalId == externalId, ct);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        context.Set<TEntity>().Add(entity);
        await context.SaveChangesAsync(ct);

        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        var existing = await GetTrackedOrThrowAsync(context, entity.Id, ct);

        context.Entry(existing).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(ct);

        return existing;
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        var entity = await GetTrackedOrThrowAsync(context, id, ct);

        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }
    
    protected static async Task<TEntity> GetTrackedOrThrowAsync(
        BafDbContext context,
        Guid id,
        CancellationToken ct = default)
    {
        var entity = await context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id, ct);

        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }
}