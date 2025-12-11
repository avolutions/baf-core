using System.Reflection;
using Avolutions.Baf.Core.Audit.Interceptors;
using Avolutions.Baf.Core.Entity.Interceptors;
using Avolutions.Baf.Core.Lookups.Interceptors;
using Avolutions.Baf.Core.Module.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Avolutions.Baf.Core.Module.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> 
/// to register BAF modules and their services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans the specified assemblies (or all loaded assemblies if none provided) 
    /// for types implementing <see cref="IFeatureModule"/>.
    /// 
    /// Each discovered module's <see cref="IFeatureModule.Register"/> method is called 
    /// to add its services to the DI container.
    /// 
    /// The discovered modules are stored in a <see cref="BafRegistry"/> singleton 
    /// for later use in <c>UseBaf()</c>.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <param name="assemblies">
    /// Optional list of assemblies to scan. If empty, all currently loaded assemblies are scanned.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddBafCore<TContext>(this IServiceCollection services, params Assembly[] assemblies)
        where TContext : BafDbContext
    {
        services.TryAddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());
        services.TryAddScoped<BafDbContext>(sp => sp.GetRequiredService<TContext>());

        var (modules, moduleAssemblies) = DiscoverModulesAndAssemblies(assemblies);

        // Let each module register its own services
        foreach (var module in modules)
        {
            module.Register(services, moduleAssemblies);
        }

        // Store discovered modules and their assemblies for later use
        services.AddSingleton(new BafRegistry(modules, moduleAssemblies));
        
        // Add database context interceptors
        services.AddDbContext<TContext>((sp, options) =>
        {
            options.AddInterceptors(
                sp.GetRequiredService<AuditSaveChangesInterceptor>(),
                sp.GetRequiredService<TrackableSaveChangesInterceptor>(),
                sp.GetRequiredService<LookupHydrationInterceptor>(),
                sp.GetRequiredService<LookupSaveChangesInterceptor>()
            );
        });
        
        return services;
    }
    
    /// <summary>
    /// Finds all concrete types implementing IFeatureModule and returns the created
    /// instances plus the distinct assemblies that contain at least one such module.
    /// </summary>
    private static (IFeatureModule[] Modules, Assembly[] Assemblies)
        DiscoverModulesAndAssemblies(Assembly[]? assemblies)
    {
        assemblies ??= [];
        if (assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        var moduleTypes = assemblies
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t is not null)!;
                }
            })
            .Where(t => t is not null
                        && typeof(IFeatureModule).IsAssignableFrom(t)
                        && !t!.IsAbstract
                        && !t.IsInterface)
            .ToArray()!;

        var modules = moduleTypes
                .Select(t => Activator.CreateInstance(t!) as IFeatureModule)
                .Where(m => m is not null)
                .Cast<IFeatureModule>()
                .ToArray();
            
        var distinctAssemblies = moduleTypes
            .Select(t => t.Assembly)
            .Distinct()
            .ToArray();
        
        return (modules, distinctAssemblies);
    }
}