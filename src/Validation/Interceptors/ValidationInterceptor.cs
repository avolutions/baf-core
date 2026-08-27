using Avolutions.Baf.Core.Entity.Abstractions;
using Avolutions.Baf.Core.Entity.Exceptions;
using Avolutions.Baf.Core.Validation.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Validation.Interceptors;

public sealed class ValidationInterceptor(IServiceScopeFactory scopeFactory)
    : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        if (eventData.Context is null)
        {
            return result;
        }

        var entries = eventData.Context.ChangeTracker
            .Entries<IEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        if (entries.Count == 0)
        {
            return result;
        }

        using var scope = scopeFactory.CreateScope();

        foreach (var entry in entries)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(entry.Metadata.ClrType);

            if (scope.ServiceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var ruleSet = entry.State == EntityState.Added
                ? RuleSets.Create
                : RuleSets.Update;

            var validationContext = ValidationContext<object>.CreateWithOptions(
                entry.Entity,
                opts =>
                {
                    opts.IncludeRuleSets(ruleSet, RuleSets.CreateOrUpdate);
                    opts.IncludeRulesNotInRuleSet();
                });

            var validationResult = await validator.ValidateAsync(validationContext, ct);

            if (!validationResult.IsValid)
            {
                throw new EntityValidationException(
                    entry.Metadata.ClrType,
                    validationResult.Errors);
            }
        }

        return result;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        throw new NotSupportedException(
            $"{nameof(ValidationInterceptor)} requires SaveChangesAsync. " +
            "Synchronous SaveChanges cannot run asynchronous validation rules.");
    }
}