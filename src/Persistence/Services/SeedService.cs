using Avolutions.Baf.Core.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence.Services;

public abstract class SeedService<T> : ISeedService<T> where T : class
{
    protected abstract IEnumerable<T> Data { get; }

    public virtual bool ShouldSeed(DbContext context)
    {
        return !context.Set<T>().Any();
    }

    public virtual async Task<bool> ShouldSeedAsync(DbContext context, CancellationToken cancellationToken = default)
    {
        return !await context.Set<T>().AnyAsync(cancellationToken);
    }

    public void Seed(DbContext context)
    {
        if (!ShouldSeed(context))
        {
            return;
        }
        context.AddRange(Data);
    }

    public async Task SeedAsync(DbContext context, CancellationToken cancellationToken = default)
    {
        if (!await ShouldSeedAsync(context, cancellationToken))
        {
            return;
        }
        await context.AddRangeAsync(Data, cancellationToken);
    }
}