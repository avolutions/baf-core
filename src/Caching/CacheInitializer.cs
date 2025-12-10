using Avolutions.Baf.Core.Caching.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Avolutions.Baf.Core.Caching;

public class CacheInitializer : IHostedService
{
    private readonly IEnumerable<ICache> _caches;
    private readonly ILogger<CacheInitializer> _logger;

    public CacheInitializer(IEnumerable<ICache> caches, ILogger<CacheInitializer> logger)
    {
        _caches = caches;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing {Count} caches...", _caches.Count());

        foreach (var cache in _caches)
        {
            var cacheName = cache.GetType().Name;
            try
            {
                _logger.LogDebug("Loading cache: {CacheName}", cacheName);
                await cache.RefreshAsync(cancellationToken);
                _logger.LogDebug("Cache loaded: {CacheName}", cacheName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load cache: {CacheName}", cacheName);
            }
        }

        _logger.LogInformation("Cache initialization completed");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}