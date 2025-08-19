using FluentValidation.Results;

namespace Avolutions.Baf.Core.Entity.Exceptions;

public sealed class EntityValidationException : Exception
{
    public Type EntityType { get; }
    public IReadOnlyList<ValidationFailure> Failures { get; }

    public EntityValidationException(Type entityType, IEnumerable<ValidationFailure> failures)
        : base($"Validation failed for {entityType.Name}.")
    {
        EntityType = entityType;
        Failures = failures.ToList();
    }
}