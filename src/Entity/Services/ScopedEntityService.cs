using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Entity.Services;

public class ScopedEntityService<TEntity> : BaseEntityService<TEntity>
    where TEntity : class, IEntity
{
    private readonly BafDbContext _context;

    public ScopedEntityService(BafDbContext context)
        : this(context, null)
    {
    }

    public ScopedEntityService(
        BafDbContext context,
        IValidator<TEntity>? validator)
        : base(validator)
    {
        _context = context;
    }

    protected override bool OwnsContext => false;

    protected override ValueTask<DbContext> GetContextAsync(CancellationToken ct)
    {
        return ValueTask.FromResult<DbContext>(_context);
    }
}