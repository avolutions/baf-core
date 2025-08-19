using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Validation.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> UniqueFor<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        DbContext dbContext,
        Func<T, Guid> currentIdSelector
    ) where T : class
    {
        return ruleBuilder.MustAsync(async (
            T instance,
            TProperty value,
            ValidationContext<T> context,
            CancellationToken cancellationToken) =>
        {
            var propertyName = context.PropertyPath;

            var parameter = Expression.Parameter(typeof(T), "e");
            var propertyAccess = Expression.PropertyOrField(parameter, propertyName);
            var valueConstant = Expression.Constant(value);
            var equality = Expression.Equal(propertyAccess, valueConstant);

            var idAccess = Expression.Property(parameter, "Id");
            var currentId = Expression.Constant(currentIdSelector(instance));
            var notEqualId = Expression.NotEqual(idAccess, currentId);

            var combined = Expression.AndAlso(equality, notEqualId);
            var lambda = Expression.Lambda<Func<T, bool>>(combined, parameter);

            return !await dbContext.Set<T>().AnyAsync(lambda, cancellationToken);
        })
        .WithMessage("'{PropertyName}' ist bereits vergeben.");
    }
}
