using System.Reflection;
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
    /// The discovered modules are stored in a <see cref="BafModuleCatalog"/> singleton 
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
        
        var modules = DiscoverModules(assemblies).ToArray();

        // Let each module register its own services
        foreach (var module in modules)
        {
            module.Register(services);
        }

        // Store discovered modules for UseBaf()
        services.AddSingleton(new BafModuleCatalog(modules));

        return services;
    }
    
    /// <summary>
    /// Finds all concrete, non-abstract, non-interface types that implement <see cref="IFeatureModule"/>.
    /// </summary>
    /// <param name="assemblies">
    /// Assemblies to search; if null or empty, uses all loaded assemblies in the current AppDomain.
    /// </param>
    private static IEnumerable<IFeatureModule> DiscoverModules(Assembly[]? assemblies)
    {
        assemblies ??= [];
        if (assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        return assemblies
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException ex) 
                { 
                    // If some types can't be loaded, skip the null ones
                    return ex.Types.Where(t => t is not null)!; 
                }
            })
            .Where(t => t is not null
                        && typeof(IFeatureModule).IsAssignableFrom(t)
                        && !t!.IsAbstract 
                        && !t.IsInterface)
            .Select(t => Activator.CreateInstance(t!) as IFeatureModule)
            .Where(m => m is not null)!;
    }

    /// <summary>
    /// Internal catalog storing all discovered modules for later retrieval.
    /// </summary>
    internal sealed record BafModuleCatalog(IFeatureModule[] Modules);
}