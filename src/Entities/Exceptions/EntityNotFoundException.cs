namespace Avolutions.BAF.Core.Entities.Exceptions;

public sealed class EntityNotFoundException : Exception
{
    public EntityNotFoundException(Type entityType, Guid id)
        : base($"{entityType.Name} with Id '{id}' was not found.") {}
}