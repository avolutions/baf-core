using System.Globalization;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Exceptions;
using Avolutions.Baf.Core.Localization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Services;

public class TranslatableEntityService<T, TTranslation> : EntityService<T>, ITranslatableEntityService<T, TTranslation>
    where T : class, ITranslatable<TTranslation>, IEntity
    where TTranslation : class, ITranslation
{
    public TranslatableEntityService(DbContext context) : base(context)
    {
    }

    public TranslatableEntityService(DbContext context, IValidator<T>? validator) : base(context, validator)
    {
    }

    public override async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (!await DbSet.AnyAsync(cancellationToken))
        {
            entity.IsDefault = true;
        }
        
        return await base.CreateAsync(entity, cancellationToken);
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
    
        // Clear current default and set new one in single operation
        await DbSet
            .Where(q => q.IsDefault || q.Id == id)
            .ExecuteUpdateAsync(
                q => q.SetProperty(x => x.IsDefault, x => x.Id == id), 
                cancellationToken);
    }

    public Task<T> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .SingleAsync(p => p.IsDefault, cancellationToken);
    }
}