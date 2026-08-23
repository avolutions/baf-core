using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Services;

public class FactoryEntityService<TEntity> : BaseEntityService<TEntity>
    where TEntity : class, IEntity
{
    protected readonly IDbContextFactory<BafDbContext> ContextFactory;

    public FactoryEntityService(IDbContextFactory<BafDbContext> contextFactory)
        : this(contextFactory, null)
    {
    }

    public FactoryEntityService(
        IDbContextFactory<BafDbContext> contextFactory,
        IValidator<TEntity>? validator)
        : base(validator)
    {
        ContextFactory = contextFactory;
    }

    protected override bool OwnsContext => true;

    protected override async ValueTask<DbContext> GetContextAsync(CancellationToken ct)
    {
        return await ContextFactory.CreateDbContextAsync(ct);
    }
}