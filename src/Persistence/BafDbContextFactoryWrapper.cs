using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence;

internal class BafDbContextFactoryWrapper<TContext> : IDbContextFactory<BafDbContext>
    where TContext : BafDbContext
{
    private readonly IDbContextFactory<TContext> _inner;

    public BafDbContextFactoryWrapper(IDbContextFactory<TContext> inner)
    {
        _inner = inner;
    }

    public BafDbContext CreateDbContext()
    {
        return _inner.CreateDbContext();
    }
}