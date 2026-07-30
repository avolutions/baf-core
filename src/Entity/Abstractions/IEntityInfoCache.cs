using Avolutions.Baf.Core.Caching.Abstractions;
using Avolutions.Baf.Core.Entity.Models;

namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface IEntityInfoCache : ICache<string, EntityInfo>;