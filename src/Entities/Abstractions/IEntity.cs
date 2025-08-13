namespace Avolutions.BAF.Core.Entities.Abstractions;

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
    /// Timestamp when the entity was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User ID of the creator.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Timestamp when the entity was last modified (UTC).
    /// </summary>
    public  DateTime ModifiedAt { get; set; }

    /// <summary>
    /// User ID of the last modifier.
    /// </summary>
    public Guid ModifiedBy { get; set; }

    /// <summary>
    /// Returns a human-readable name for the entity.
    /// </summary>
    public string GetName() => string.Empty;
}