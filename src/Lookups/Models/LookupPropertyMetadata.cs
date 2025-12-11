namespace Avolutions.Baf.Core.Lookups.Models;

public class LookupPropertyMetadata
{
    public required Func<object, Guid> GetId { get; init; }
    public required Action<object, object?> SetLookup { get; init; }
    public required Func<Guid, object?> GetFromCache { get; init; }
}