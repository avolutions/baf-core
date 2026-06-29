using System.Reflection;
using Avolutions.Baf.Core.Persistence.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Avolutions.Baf.Core.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    private static readonly MethodInfo ApplyConfigurationMethod =
        typeof(ModelBuilder).GetMethods()
            .Single(m => m is { Name: nameof(ModelBuilder.ApplyConfiguration), IsGenericMethodDefinition: true }
                         && m.GetParameters().Length == 1);

    
    public static void ApplyModelConfigurationsFromAssembly(
        this ModelBuilder modelBuilder,
        Assembly assembly)
    {
        var types = assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IModelConfiguration).IsAssignableFrom(t));

        foreach (var type in types)
        {
            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                continue;
            }

            if (Activator.CreateInstance(type) is IModelConfiguration modelConfiguration)
            {
                modelConfiguration.Configure(modelBuilder);
            }
        }
    }
    
    /// <summary>
    /// Runs each <see cref="IEntityTypeConfiguration{TEntity}"/> in the assembly, but only
    /// for entity types that are actually registered via a DbSet. This prevents unused
    /// framework entities from being mapped (and becoming tables), and avoids mapping a
    /// base type alongside its registered derived type (which would force a discriminator).
    /// </summary>
    public static void ApplyConfigurationsForRegisteredTypes(
        this ModelBuilder modelBuilder,
        Assembly assembly,
        IReadOnlySet<Type> registeredEntityTypes)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type is not { IsAbstract: false, IsInterface: false })
            {
                continue;
            }

            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                continue;
            }

            // Entity types this configuration targets that are actually registered.
            // A config inheriting a generic base (e.g. ArticleConfiguration<AppArticle>)
            // exposes IEntityTypeConfiguration<AppArticle> AND IEntityTypeConfiguration<Article>;
            // filtering to registered types drops the unregistered base.
            var targets = type.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                .Select(i => i.GetGenericArguments()[0])
                .Where(registeredEntityTypes.Contains)
                .ToList();

            if (targets.Count == 0)
            {
                continue;
            }

            // Apply once, against the most-derived registered target.
            var entityType = targets.OrderByDescending(CountBaseTypes).First();

            var configuration = Activator.CreateInstance(type)!;
            ApplyConfigurationMethod
                .MakeGenericMethod(entityType)
                .Invoke(modelBuilder, [configuration]);
        }
    }

    /// <summary>
    /// The entity CLR types registered on the context via DbSet&lt;T&gt; properties,
    /// across the whole inheritance chain (app context, BafDbContext, Identity).
    /// </summary>
    public static IReadOnlySet<Type> GetRegisteredEntityTypes(this DbContext context)
    {
        return context.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .ToHashSet();
    }
    
    private static int CountBaseTypes(Type type)
    {
        var count = 0;
        
        for (var b = type.BaseType; b is not null; b = b.BaseType)
        {
            count++;
        }
        
        return count;
    }
}