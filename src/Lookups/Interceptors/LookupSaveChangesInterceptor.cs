using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Avolutions.Baf.Core.Lookups.Interceptors;

public class LookupSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ILookupHydrator _hydrator;

    public LookupSaveChangesInterceptor(ILookupHydrator hydrator)
    {
        _hydrator = hydrator;
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        HydrateEntities(eventData.Context);
        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        HydrateEntities(eventData.Context);
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void HydrateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is IEntity)
            .Where(e => e.Entity is not ILookup)
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            _hydrator.Hydrate((IEntity)entry.Entity);
        }
    }
}