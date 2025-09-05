using Microsoft.Extensions.Hosting;

namespace Avolutions.Baf.Core.Identity.Caching;

public class UserCacheInitializer : IHostedService
{
    private readonly IUserCache _cache;

    public UserCacheInitializer(IUserCache cache)
    {
        _cache = cache;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _cache.RefreshAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}