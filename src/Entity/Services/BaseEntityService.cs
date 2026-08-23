using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Exceptions;
using Avolutions.Baf.Core.Persistence;
using Avolutions.Baf.Core.Validation.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Services;

public abstract class BaseEntityService<TEntity> : IEntityService<TEntity>
    where TEntity : class, IEntity
{
    protected readonly IValidator<TEntity>? Validator;

    protected BaseEntityService(IValidator<TEntity>? validator)
    {
        Validator = validator;
    }

    /// <summary>Where the context comes from. The only thing the flavors decide.</summary>
    protected abstract ValueTask<DbContext> GetContextAsync(CancellationToken ct);

    /// <summary>True when this service created the context and must dispose it.</summary>
    protected abstract bool OwnsContext { get; }

    /// <summary>
    /// A context plus the knowledge of whether to dispose it. Internal to this
    /// class — callers never see it.
    /// </summary>
    protected async Task<ContextScope> UseContextAsync(CancellationToken ct)
    {
        return new ContextScope(await GetContextAsync(ct), OwnsContext);
    }

    protected readonly struct ContextScope : IAsyncDisposable
    {
        private readonly bool _owned;

        public DbContext Context { get; }

        public DbSet<TEntity> Set => Context.Set<TEntity>();

        public ContextScope(DbContext context, bool owned)
        {
            Context = context;
            _owned = owned;
        }

        public ValueTask DisposeAsync()
        {
            return !_owned ? ValueTask.CompletedTask : Context.DisposeAsync();
        }
    }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        await using var scope = await UseContextAsync(ct);

        return await ApplyIncludes(scope.Set.AsNoTracking())
            .ToListAsync(ct);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var scope = await UseContextAsync(ct);

        return await ApplyIncludes(scope.Set.AsNoTracking())
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public virtual async Task<TEntity?> GetByExternalIdAsync(string externalId, CancellationToken ct = default)
    {
        await using var scope = await UseContextAsync(ct);

        return await ApplyIncludes(scope.Set.AsNoTracking())
            .FirstOrDefaultAsync(e => e.ExternalId == externalId, ct);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        await using var scope = await UseContextAsync(ct);

        await ValidateOrThrowAsync(entity, RuleSets.Create, ct);

        scope.Set.Add(entity);
        await scope.Context.SaveChangesAsync(ct);

        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        await using var scope = await UseContextAsync(ct);

        await ValidateOrThrowAsync(entity, RuleSets.Update, ct);

        var existing = await GetTrackedOrThrowAsync(scope, entity.Id, ct);
        scope.Context.Entry(existing).CurrentValues.SetValues(entity);

        await scope.Context.SaveChangesAsync(ct);

        return existing;
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await using var scope = await UseContextAsync(ct);

        var entity = await GetTrackedOrThrowAsync(scope, id, ct);

        scope.Set.Remove(entity);
        await scope.Context.SaveChangesAsync(ct);
    }
    
    protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)
    {
        return query;
    }

    protected virtual async Task<TEntity> GetTrackedOrThrowAsync(
        ContextScope scope,
        Guid id,
        CancellationToken ct = default)
    {
        var entity = await ApplyIncludes(scope.Set)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    protected virtual async Task ValidateOrThrowAsync(
        TEntity entity,
        string? ruleSet = null,
        CancellationToken ct = default)
    {
        if (Validator is null)
        {
            return;
        }

        var result = await Validator.ValidateAsync(entity, opts =>
        {
            if (!string.IsNullOrWhiteSpace(ruleSet))
            {
                opts.IncludeRuleSets(ruleSet, RuleSets.CreateOrUpdate);
            }

            opts.IncludeRulesNotInRuleSet();
        }, ct);

        if (!result.IsValid)
        {
            throw new EntityValidationException(typeof(TEntity), result.Errors);
        }
    }
}