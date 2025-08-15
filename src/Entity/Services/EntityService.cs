using Avolutions.BAF.Core.Entity.Abstractions;
using Avolutions.BAF.Core.Entity.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.BAF.Core.Entity.Services;

public class EntityService<TEntity> : IEntityService<TEntity>
    where TEntity : class, IEntity
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;
        
    public EntityService(DbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }
        
    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        DbSet.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var exists = await DbSet.AnyAsync(e => e.Id == entity.Id);
        if (!exists)
        {
            throw new EntityNotFoundException(typeof(TEntity), entity.Id);
        }

        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public virtual Task<Guid?> GetPreviousIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public virtual Task<Guid?> GetNextIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<TEntity?> GetByExternalIdAsync(string externalId)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.ExternalId == externalId);
    }
}