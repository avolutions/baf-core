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

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        HydrateEntities(eventData.Context);
        NeutralizeLookups(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        HydrateEntities(eventData.Context);
        NeutralizeLookups(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void HydrateEntities(DbContext? context)
    {
        if (context is null)
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

    private static void NeutralizeLookups(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is ILookup or ILookupTranslation)
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
        
        foreach (var entry in entries)
        {
            entry.State = EntityState.Unchanged;
        }
    }
}