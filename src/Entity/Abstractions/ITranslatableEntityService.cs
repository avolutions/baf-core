namespace Avolutions.Baf.Core.Entity.Abstractions;

public interface ITranslatableEntityService<T, TTranslation> : IEntityService<T>
    where T : class, ITranslatable<T, TTranslation>, IEntity
    where TTranslation : class, ITranslation<T>
{
    Task<T?> GetByIdAsync(Guid id, string language, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(string language, CancellationToken cancellationToken = default);
}