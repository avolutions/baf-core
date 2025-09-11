namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface IEntityService<TEntity>
    where TEntity : IEntity
{
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id);
    Task<TEntity?> GetByExternalIdAsync(string externalId);
}