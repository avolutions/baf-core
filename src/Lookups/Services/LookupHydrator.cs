using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Lookups.Abstractions;

namespace Avolutions.Baf.Core.Lookups.Services;

public class LookupHydrator : ILookupHydrator
{
    private readonly ILookupHydrationCache _hydrationCache;

    public LookupHydrator(ILookupHydrationCache hydrationCache)
    {
        _hydrationCache = hydrationCache;
    }

    public void Hydrate(IEntity entity)
    {
        var metadata = _hydrationCache.GetMetadata(entity.GetType());
        if (metadata == null)
        {
            return;
        }

        foreach (var prop in metadata)
        {
            var id = prop.GetId(entity);
            if (id == Guid.Empty)
            {
                continue;
            }

            var lookup = prop.GetFromCache(id);
            if (lookup != null)
            {
                prop.SetLookup(entity, lookup);
            }
        }
    }
}