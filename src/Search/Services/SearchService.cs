using Avolutions.Baf.Core.Search.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Search.Services;

public class SearchService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SearchService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<SearchResult>> SearchAsync(string term, int max = 25,
        CancellationToken cancellationToken = default)
    {
        var results = new List<SearchResult>();

        using var discoveryScope = _scopeFactory.CreateScope();
        var services = discoveryScope.ServiceProvider.GetServices<ISearchService>();

        foreach (var service in services)
        {
            var partial = await service.SearchAsync(term, cancellationToken);
            results.AddRange(partial);

            if (results.Count >= max)
            {
                break;
            }
        }

        return results.Take(max).ToList();
    }
}