using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            .Where(e => e.Entity is not ILookup)
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            _hydrator.Hydrate(entry.Entity);
        }
    }

    private static void NeutralizeLookups(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var neutralizedLookups = new HashSet<object>(ReferenceEqualityComparer.Instance);

        var parentEntries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is not (ILookup or ILookupTranslation));

        foreach (var parent in parentEntries)
        {
            foreach (var nav in parent.Navigations)
            {
                CollectReferencedLookups(nav, neutralizedLookups);
            }
        }

        // Neutralize the referenced lookups, and record their keys.
        var neutralizedKeys = new HashSet<Guid>();

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(e => e.Entity is ILookup)
                     .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                     .Where(e => neutralizedLookups.Contains(e.Entity)))
        {
            entry.State = EntityState.Unchanged;
            neutralizedKeys.Add(((IEntity)entry.Entity).Id);
        }

        // A translation follows its lookup. Match by instance OR by FK value.
        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(e => e.Entity is ILookupTranslation)
                     .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            var owningLookup = entry.References
                .Select(r => r.CurrentValue)
                .FirstOrDefault(v => v is ILookup);

            var matchedByInstance = owningLookup is not null && neutralizedLookups.Contains(owningLookup);

            var matchedByKey = TryGetLookupForeignKey(entry, out var fk) && neutralizedKeys.Contains(fk);

            if (matchedByInstance || matchedByKey)
            {
                entry.State = EntityState.Unchanged;
            }
        }
    }
    
    private static bool TryGetLookupForeignKey(EntityEntry entry, out Guid foreignKey)
    {
        foreach (var fk in entry.Metadata.GetForeignKeys())
        {
            // The FK whose principal is the lookup side.
            if (typeof(ILookup).IsAssignableFrom(fk.PrincipalEntityType.ClrType))
            {
                var prop = fk.Properties[0];
                var value = entry.Property(prop.Name).CurrentValue;

                if (value is Guid guid)
                {
                    foreignKey = guid;
                    return true;
                }
            }
        }

        foreignKey = Guid.Empty;
        return false;
    }

    private static void CollectReferencedLookups(NavigationEntry nav, HashSet<object> sink)
    {
        if (nav is ReferenceEntry reference)
        {
            if (reference.CurrentValue is ILookup)
            {
                sink.Add(reference.CurrentValue);
            }
        }
        else if (nav is CollectionEntry { CurrentValue: not null } collection)
        {
            foreach (var item in collection.CurrentValue)
            {
                if (item is ILookup)
                {
                    sink.Add(item);
                }
            }
        }
    }
}