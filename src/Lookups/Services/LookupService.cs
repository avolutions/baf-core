using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Services;
using Avolutions.Baf.Core.Localization;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Avolutions.Baf.Core.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Lookups.Services;

public class LookupService<T, TTranslation> : BaseEntityService<T>, ILookupService<T>
    where T : class, ILookup<TTranslation>, IEntity
    where TTranslation : class, ILookupTranslation
{
    private readonly ILookupCache<T>? _cache;
    
    public LookupService(
        IDbContextFactory<BafDbContext> contextFactory,
        ILookupCache<T>? cache = null) : base(contextFactory)
    {
        _cache = cache;
    }

    public override async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);
        
        if (!await context.Set<T>().AnyAsync(ct))
        {
            entity.IsDefault = true;
        }
        
        var result = await base.CreateAsync(entity, ct);
        await RefreshCacheAsync(ct);
        
        return result;
    }
    
    public override async Task<T> UpdateAsync(T entity, CancellationToken ct = default)
    {
        var result = await base.UpdateAsync(entity, ct);
        await RefreshCacheAsync(ct);
        return result;
    }

    public override async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await base.DeleteAsync(id, ct);
        await RefreshCacheAsync(ct);
    }

    public override async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await GetByIdAsync(id, LocalizationContext.CurrentLanguage, ct);
    }

    public async Task<T?> GetByIdAsync(Guid id, string language, CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        return await context.Set<T>()
            .Include(p => p.Translations.Where(t => t.Language == language))
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, ct);
    }

    public override async Task<List<T>> GetAllAsync(CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        return await context.Set<T>()
            .Include(p => p.Translations)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<List<T>> GetAllAsync(string language, CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);

        return await context.Set<T>()
            .Include(p => p.Translations.Where(t => t.Language == language))
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task SetDefaultAsync(Guid id, CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);
        var dbSet = context.Set<T>();
        
        // Check if the entity exists
        await GetByIdOrThrowAsync(context, id, ct);
        
        var isAlreadyDefault = await dbSet.AnyAsync(e => e.Id == id && e.IsDefault, ct);
        if (isAlreadyDefault)
        {
            return;
        }
        
        // Clear current default
        await dbSet
            .Where(q => q.IsDefault)
            .ExecuteUpdateAsync(
                q => q.SetProperty(x => x.IsDefault, false),
                ct);

        // Set new default
        await dbSet
            .Where(q => q.Id == id)
            .ExecuteUpdateAsync(
                q => q.SetProperty(x => x.IsDefault, true),
                ct);

        await RefreshCacheAsync(ct);
    }

    public async Task<T> GetDefaultAsync(CancellationToken ct = default)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(ct);
        
        return await context.Set<T>()
            .AsNoTracking()
            .SingleAsync(p => p.IsDefault, ct);
    }
    
    private async Task RefreshCacheAsync(CancellationToken ct = default)
    {
        if (_cache is not null)
        {
            await _cache.RefreshAsync(ct);
        }
    }
}