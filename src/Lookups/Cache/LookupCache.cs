using Avolutions.Baf.Core.Caching;
using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Lookups.Cache;

public class LookupCache<T, TTranslation> : CacheBase<T>, ILookupCache<T>
    where T : class, ILookup<TTranslation>, IEntity
    where TTranslation : class, ILookupTranslation
{
    private T? _default;

    public LookupCache(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }

    public Task<T> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _default ?? throw new InvalidOperationException($"No default {typeof(T).Name} configured"));
    }

    public Task<Guid> GetDefaultIdAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _default?.Id ?? throw new InvalidOperationException($"No default {typeof(T).Name} configured"));
    }

    protected override async Task<IReadOnlyList<T>> LoadAsync(CancellationToken cancellationToken)
    {
        using var scope = ScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();

        var items = await context.Set<T>()
            .Include(e => e.Translations)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        _default = items.FirstOrDefault(x => x.IsDefault) ?? items.FirstOrDefault();

        return items;
    }

    protected override Guid GetId(T item) => item.Id;
}