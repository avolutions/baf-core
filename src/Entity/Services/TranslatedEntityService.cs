using System.Globalization;
using Avolutions.Baf.Core.Entity.Abstractions;
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

    public override async Task<List<T>> GetAllAsync()
    {
        return await DbSet
            .Include(p => p.Translations)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(string language, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Translations.Where(t => t.Language == language))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}