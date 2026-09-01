namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface IEntityService<TEntity>
    where TEntity : IEntity
{
    Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<TEntity?> GetByExternalIdAsync(string externalId, CancellationToken ct = default);
}