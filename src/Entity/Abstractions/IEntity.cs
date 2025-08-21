namespace Avolutions.Baf.Core.Entity.Abstractions;

/// <summary>
/// Base contract for all entities in BAF.
/// Provides identity, audit fields, and a display name hook.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Internal unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Optional external identifier from another system.
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Returns a human-readable name for the entity.
    /// </summary>
    public string GetName() => string.Empty;
}