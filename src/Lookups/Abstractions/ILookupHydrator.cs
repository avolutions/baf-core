using Avolutions.Baf.Core.Entity.Abstractions;

namespace Avolutions.Baf.Core.Lookups.Abstractions;

public interface ILookupHydrator
{
    void Hydrate(IEntity entity);
}