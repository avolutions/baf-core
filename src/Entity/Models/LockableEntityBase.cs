using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Entity.Models;

public abstract class LockableEntityBase : EntityBase, ILockable
{
    public EntityLockStatus LockStatus { get; set; } = new();
}