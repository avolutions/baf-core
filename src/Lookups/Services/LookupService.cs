using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Exceptions;
using Avolutions.Baf.Core.Entity.Services;
using Avolutions.Baf.Core.Localization;
using Avolutions.Baf.Core.Lookups.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Lookups.Services;

public class LookupService<T, TTranslation> : EntityService<T>, ILookupService<T>
    where T : class, ILookup<TTranslation>, IEntity
    where TTranslation : class, ILookupTranslation
{
    private readonly ILookupCache<T>? _cache;
    
    public LookupService(
        DbContext context,
        ILookupCache<T>? cache = null,
        IValidator<T>? validator = null) : base(context, validator)
    {
        _cache = cache;
    }

    public override async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (!await DbSet.AnyAsync(cancellationToken))
        {
            entity.IsDefault = true;
        }
        
        var result = await base.CreateAsync(entity, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
        
        return result;
    }
    
    public override async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var result = await base.UpdateAsync(entity, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
        return result;
    }

    public override async Task DeleteAsync(Guid id)
    {
        await base.DeleteAsync(id);
        await RefreshCacheAsync();
    }

    public override async Task<T?> GetByIdAsync(Guid id)
    {
        return await GetByIdAsync(id, LocalizationContext.CurrentLanguage);
    }

    public async Task<T?> GetByIdAsync(Guid id, string language, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Translations.Where(t => t.Language == language))
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Translations)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(string language, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Translations.Where(t => t.Language == language))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task SetDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await DbSet.AnyAsync(e => e.Id == id, cancellationToken);
        if (!exists)
        {
            throw new EntityNotFoundException(typeof(T), id);
        }

        var isAlreadyDefault = await DbSet.AnyAsync(e => e.Id == id && e.IsDefault, cancellationToken);
        if (isAlreadyDefault)
        {
            return;
        }

        // Clear current default
        await DbSet
            .Where(q => q.IsDefault)
            .ExecuteUpdateAsync(
                q => q.SetProperty(x => x.IsDefault, false),
                cancellationToken);

        // Set new default
        await DbSet
            .Where(q => q.Id == id)
            .ExecuteUpdateAsync(
                q => q.SetProperty(x => x.IsDefault, true),
                cancellationToken);

        await RefreshCacheAsync(cancellationToken);
    }

    public Task<T> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .SingleAsync(p => p.IsDefault, cancellationToken);
    }
    
    private async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        if (_cache is not null)
        {
            await _cache.RefreshAsync(cancellationToken);
        }
    }
}