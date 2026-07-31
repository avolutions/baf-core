namespace Avolutions.Baf.Core.Entity.Models;

public class EntityLockStatus
{
    public EntityLockLevel Level { get; set; } = EntityLockLevel.None;
    public string? Message { get; set; }

    public bool IsSet => Level != EntityLockLevel.None;

    public bool IsBlocked => Level == EntityLockLevel.Block;
}