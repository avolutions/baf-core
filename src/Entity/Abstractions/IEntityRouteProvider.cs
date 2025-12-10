namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface IEntityRouteProvider<TEntity> where TEntity : class, IEntity
{
    string Index { get; }
    string Create { get; }
    string Details(Guid id);
    string Edit(Guid id);
}