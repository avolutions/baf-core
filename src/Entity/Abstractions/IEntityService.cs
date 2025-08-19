namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface IEntityService<TEntity>
    where TEntity : IEntity
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
    Task<Guid?> GetPreviousIdAsync(Guid id);
    Task<Guid?> GetNextIdAsync(Guid id);
    Task<TEntity?> GetByExternalIdAsync(string externalId);
}