namespace Avolutions.Baf.Core.Entity.Models;

public record EntityNavigationResult(
    Guid? FirstId,
    Guid? PreviousId,
    Guid? NextId,
    Guid? LastId,
    int? CurrentIndex,
    int TotalCount)
{
    public bool HasFirst => CurrentIndex > 0;
    public bool HasPrevious => PreviousId.HasValue;
    public bool HasNext => NextId.HasValue;
    public bool HasLast => CurrentIndex < TotalCount - 1;
}