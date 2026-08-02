using Avolutions.Baf.Core.Entity.Models;

namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ILockable
{
    EntityLockStatus LockStatus { get; set; }
}