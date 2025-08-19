using Avolutions.Baf.Core.Search.Models;

namespace Avolutions.Baf.Core.Search.Services;

public interface ISearchService
{
    Task<List<SearchResult>> SearchAsync(string term, CancellationToken cancellationToken = default);
}