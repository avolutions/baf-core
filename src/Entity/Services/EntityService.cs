using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Exceptions;
using Avolutions.Baf.Core.Validation.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Services;

public class EntityService<TEntity> : IEntityService<TEntity>
    where TEntity : class, IEntity
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly IValidator<TEntity>? Validator;

    public EntityService(DbContext context) : this(context, null) {}
        
    public EntityService(DbContext context, IValidator<TEntity>? validator)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
        Validator = validator;
    }
        
    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await ValidateOrThrowAsync(entity, RuleSets.Create, cancellationToken);
        
        DbSet.Add(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var exists = await DbSet.AnyAsync(e => e.Id == entity.Id, cancellationToken: cancellationToken);
        if (!exists)
        {
            throw new EntityNotFoundException(typeof(TEntity), entity.Id);
        }
        
        await ValidateOrThrowAsync(entity, RuleSets.Update, cancellationToken);

        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
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

    public virtual async Task<TEntity?> GetByExternalIdAsync(string externalId, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(e => e.ExternalId == externalId, cancellationToken: ct);
    }
    
    protected virtual async Task ValidateOrThrowAsync(TEntity entity, string? ruleSet = null, CancellationToken ct = default)
    {
        if (Validator is null) return; // no validator registered for TEntity

        ValidationResult result = await Validator.ValidateAsync(entity, opts =>
        {
            if (!string.IsNullOrWhiteSpace(ruleSet))
            {
                opts.IncludeRuleSets(ruleSet);
            }
        }, ct);

        if (!result.IsValid)
        {
            throw new EntityValidationException(typeof(TEntity), result.Errors);
        }
    }
}