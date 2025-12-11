using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Lookups.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Avolutions.Baf.Core.Lookups.Interceptors;

public class LookupHydrationInterceptor : IMaterializationInterceptor
{
    private readonly ILookupHydrator _hydrator;

    public LookupHydrationInterceptor(ILookupHydrator hydrator)
    {
        _hydrator = hydrator;
    }

    public object InitializedInstance(MaterializationInterceptionData materializationData, object entity)
    {
        if (entity is IEntity entityBase)
        {
            _hydrator.Hydrate(entityBase);
        }
        
        return entity;
    }
}