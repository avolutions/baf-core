namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITranslatableEntityService<T, TTranslation> : IEntityService<T>
    where T : class, ITranslatable<TTranslation>, IEntity
    where TTranslation : class, ITranslation
{
    Task<T?> GetByIdAsync(Guid id, string language, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(string language, CancellationToken cancellationToken = default);
}