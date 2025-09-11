using System.Globalization;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Exceptions;
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
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        return await GetByIdAsync(id, lang);
    }

    public async Task<T?> GetByIdAsync(Guid id, string language, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Translations.Where(t => t.Language == language))
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken: cancellationToken);
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
        var entity = await DbSet.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(T), id);
        }
        
        // If it's already the default, nothing to do
        if (entity.IsDefault)
        {
            return;
        }
        
        // Remove default from all units
        await DbSet
            .Where(q => q.IsDefault)
            .ExecuteUpdateAsync(q => q.SetProperty(x => x.IsDefault, false), cancellationToken: cancellationToken);
        
        // Set the new default
        entity.IsDefault = true;
        
        await Context.SaveChangesAsync(cancellationToken);
    }

    public Task<T> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .SingleAsync(p => p.IsDefault, cancellationToken);
    }
}