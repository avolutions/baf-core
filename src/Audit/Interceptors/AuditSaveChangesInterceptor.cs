using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Avolutions.Baf.Core.Audit.Attributes;
using Avolutions.Baf.Core.Audit.Models;
using Avolutions.Baf.Core.Entity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Avolutions.Baf.Core.Audit.Interceptors;

public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private static readonly ConcurrentDictionary<Type, bool> AuditedTypeCache = new();

    // Derive trackable property names from ITrackable (no magic strings)
    private static readonly Lazy<HashSet<string>> TrackablePropertyNames =
        new Lazy<HashSet<string>>(CreateTrackablePropertyNameSet, isThreadSafe: true);

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = false
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context!;
        var auditLogs = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (!TryResolveAuditTarget(entry, out var entityName, out var entityId, out var auditAction))
            {
                continue;
            }

            var auditLog = new AuditLog
            {
                Entity = entityName,
                EntityId = entityId,
                Action = auditAction
            };

            var isCreate = auditAction == AuditAction.Created;
            var isDelete = auditAction == AuditAction.Deleted;

            if (isCreate)
            {
                auditLog.NewState = BuildSnapshotJson(entry, useOriginalValues: false, includeOnlyChanged: true);
            }
            else if (isDelete)
            {
                auditLog.OldState = BuildSnapshotJson(entry, useOriginalValues: true, includeOnlyChanged: true);
            }
            else
            {
                auditLog.OldState = BuildSnapshotJson(entry, useOriginalValues: true, includeOnlyChanged: true);
                auditLog.NewState = BuildSnapshotJson(entry, useOriginalValues: false, includeOnlyChanged: true);
            }

            var hasAnyChange =
                !string.IsNullOrEmpty(auditLog.OldState) ||
                !string.IsNullOrEmpty(auditLog.NewState);

            if (auditLog.Action != AuditAction.Updated || hasAnyChange)
            {
                auditLogs.Add(auditLog);
            }
        }

        if (auditLogs.Count > 0)
        {
            context.Set<AuditLog>().AddRange(auditLogs);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Resolve the audited subject (entity name and Guid id) and action.
    /// Only entities implementing IEntity and annotated with [Audited] are subjects.
    /// Owned types are attributed to their audited owner IEntity.
    /// </summary>
    private static bool TryResolveAuditTarget(
        EntityEntry entry,
        out string entityName,
        out Guid entityId,
        out AuditAction auditAction)
    {
        entityName = null!;
        entityId = Guid.Empty;
        auditAction = default;

        if (entry.State is EntityState.Detached or EntityState.Unchanged)
        {
            return false;
        }

        if (entry.Entity is AuditLog)
        {
            return false;
        }

        if (!TryMapEntityStateToAuditAction(entry.State, out auditAction))
        {
            return false;
        }

        var clrType = entry.Metadata.ClrType;

        if (IsAuditedType(clrType) && entry.Entity is IEntity subjectEntity)
        {
            entityName = clrType.Name;
            entityId = subjectEntity.Id;
            return true;
        }

        var ownership = entry.Metadata.FindOwnership();
        if (ownership is not null)
        {
            var ownerClrType = ownership.PrincipalEntityType.ClrType;
            var ownerImplementsIEntity = typeof(IEntity).IsAssignableFrom(ownerClrType);

            if (ownerImplementsIEntity && IsAuditedType(ownerClrType))
            {
                if (TryGetPrimaryKeyGuid(entry, out var ownerId))
                {
                    entityName = ownerClrType.Name;
                    entityId = ownerId;
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryMapEntityStateToAuditAction(EntityState state, out AuditAction auditAction)
    {
        switch (state)
        {
            case EntityState.Added:
                auditAction = AuditAction.Created;
                return true;
            case EntityState.Modified:
                auditAction = AuditAction.Updated;
                return true;
            case EntityState.Deleted:
                auditAction = AuditAction.Deleted;
                return true;
            default:
                auditAction = default;
                return false;
        }
    }

    private static bool IsAuditedType(Type clrType)
    {
        return AuditedTypeCache.GetOrAdd(
            clrType,
            HasAuditedAttribute
        );
    }

    private static bool HasAuditedAttribute(Type type)
    {
        return type.GetCustomAttribute<AuditedAttribute>(inherit: true) != null;
    }

    private static bool ShouldAuditProperty(PropertyEntry propertyEntry)
    {
        var propertyInfo = propertyEntry.Metadata.PropertyInfo;
        if (propertyInfo is null)
        {
            return false; // shadow property
        }

        if (propertyEntry.EntityEntry.Entity is ITrackable)
        {
            var propertyName = propertyEntry.Metadata.Name;
            if (TrackablePropertyNames.Value.Contains(propertyName))
            {
                return false;
            }
        }

        if (Attribute.IsDefined(propertyInfo, typeof(IgnoreAuditAttribute), inherit: true))
        {
            return false;
        }

        if (propertyEntry.Metadata.IsKey())
        {
            return false;
        }

        if (propertyEntry.Metadata.IsConcurrencyToken)
        {
            return false;
        }

        if (propertyEntry.Metadata.ValueGenerated == ValueGenerated.OnAddOrUpdate)
        {
            return false;
        }

        var propertyClrType = propertyEntry.Metadata.ClrType;
        if (propertyClrType != typeof(string) && propertyClrType.IsClass)
        {
            return false;
        }

        return true; // include FK scalars and all other scalar primitives
    }

    private static (object? oldValue, object? newValue) GetOldAndNewValues(PropertyEntry propertyEntry, EntityState entityState)
    {
        switch (entityState)
        {
            case EntityState.Added:
                return (null, propertyEntry.CurrentValue);
            case EntityState.Deleted:
                return (propertyEntry.OriginalValue, null);
            default:
                return (propertyEntry.OriginalValue, propertyEntry.CurrentValue);
        }
    }

    private static string? BuildSnapshotJson(
        EntityEntry entry,
        bool useOriginalValues,
        bool includeOnlyChanged)
    {
        var jsonObject = new JsonObject();

        foreach (var propertyEntry in entry.Properties.Where(ShouldAuditProperty))
        {
            var (oldValue, newValue) = GetOldAndNewValues(propertyEntry, entry.State);

            var changed = entry.State switch
            {
                EntityState.Added => newValue is not null,
                EntityState.Deleted => oldValue is not null,
                _ => propertyEntry.IsModified && !Equals(oldValue, newValue)
            };

            if (includeOnlyChanged && !changed)
            {
                continue;
            }

            var chosen = useOriginalValues ? oldValue : newValue;
            jsonObject[propertyEntry.Metadata.Name] =
                chosen is null ? null : JsonSerializer.SerializeToNode(chosen, JsonOptions);
        }

        if (jsonObject.Count == 0)
        {
            return null;
        }

        return jsonObject.ToJsonString(JsonOptions);
    }

    private static bool TryGetPrimaryKeyGuid(EntityEntry entry, out Guid id)
    {
        foreach (var property in entry.Properties.Where(p => p.Metadata.IsPrimaryKey()))
        {
            var value = property.CurrentValue ?? property.OriginalValue;

            if (value is Guid guid)
            {
                id = guid;
                return true;
            }

            if (value is string stringValue && Guid.TryParse(stringValue, out var parsed))
            {
                id = parsed;
                return true;
            }
        }

        id = Guid.Empty;
        return false;
    }

    private static HashSet<string> CreateTrackablePropertyNameSet()
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var properties = typeof(ITrackable).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            set.Add(property.Name);
        }

        return set;
    }
}
