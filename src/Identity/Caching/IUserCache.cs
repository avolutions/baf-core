using Avolutions.Baf.Core.Identity.Models;

namespace Avolutions.Baf.Core.Identity.Caching;

public interface IUserCache
{
    UserInfo Get(Guid userId);

    Task RefreshAsync(CancellationToken ct = default);
}
