using Avolutions.BAF.Core.Search.Models;

namespace Avolutions.BAF.Core.Search.Services;

public interface ISearchService
{
    Task<List<SearchResult>> SearchAsync(string term, CancellationToken cancellationToken = default);
}