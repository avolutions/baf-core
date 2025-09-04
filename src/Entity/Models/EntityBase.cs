using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Entity.Models;

/// <summary>
/// Base entity implementation for BAF entities.
/// Inherit from this class to get all core fields and override <see cref="GetName"/>.
/// </summary>
public abstract class EntityBase : IEntity, ITrackable
{
    public Guid Id { get; set; }
    public string? ExternalId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public virtual string GetName() => string.Empty;
}