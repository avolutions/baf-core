using Avolutions.BAF.Core.Entity.Abstractions;
using Avolutions.BAF.Core.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.BAF.Core.Entity.Hooks;

public class AuditHook : ISaveChangesHook
{
    public Task OnBeforeSaveChanges(DbContext context, CancellationToken ct)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e is { Entity: IEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entry in entries)
        {
            var entity = (IEntity)entry.Entity;
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            entity.ModifiedAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }
}