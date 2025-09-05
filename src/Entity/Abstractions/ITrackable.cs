namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITrackable
{
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
}