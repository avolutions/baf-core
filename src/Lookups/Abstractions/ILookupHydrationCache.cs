using Avolutions.Baf.Core.Caching.Abstractions;
using Avolutions.Baf.Core.Lookups.Models;

namespace Avolutions.Baf.Core.Lookups.Abstractions;

public interface ILookupHydrationCache : ICache
{
    LookupPropertyMetadata[]? GetMetadata(Type entityType);
}