namespace Avolutions.BAF.Core.Entity.Exceptions;

public sealed class EntityNotFoundException(Type entityType, Guid id)
    : Exception($"{entityType.Name} with Id '{id}' was not found.");