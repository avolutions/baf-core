using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Modules.Extensions;

/// <summary>
/// Provides extension methods for <see cref="WebApplication"/> 
/// to initialize and configure BAF modules at application startup.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Discovers and executes all <see cref="IFeatureModule"/> implementations 
    /// that were registered via <c>AddBafCore()</c>.
    /// 
    /// This method:
    /// 1. Calls <see cref="IFeatureModule.Configure"/> on each module 
    ///    to configure the middleware pipeline.
    /// 2. Creates a scoped service provider to run <see cref="IFeatureModule.InitializeAsync"/> 
    ///    for post-startup initialization logic (e.g., seeding data, building navigation).
    /// </summary>
    /// <param name="app">The application to configure.</param>
    /// <returns>The <see cref="WebApplication"/> instance for chaining.</returns>
    public static WebApplication UseBaf(this WebApplication app)
    {
        // Retrieve module catalog populated by AddBafCore()
        var catalog = app.Services.GetService<ServiceCollectionExtensions.BafModuleCatalog>();
        if (catalog is null)
        {
            // No modules registered
            return app;
        }

        // Configure middleware pipeline for each module
        foreach (var m in catalog.Modules)
        {
            m.Configure(app);
        }
        
        // Create a scoped DI context for initialization
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;

        // Initialize modules synchronously (safe during startup)
        foreach (var m in catalog.Modules)
        {
            m.InitializeAsync(sp, app.Lifetime.ApplicationStopping)
                .GetAwaiter()
                .GetResult(); // Blocking is OK here since startup is synchronous
        }

        return app;
    }
}