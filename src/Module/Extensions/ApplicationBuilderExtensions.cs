using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Module.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseBafCore(this WebApplication app)
    {
        // Retrieve module catalog populated by AddBafCore()
        var catalog = app.Services.GetService<ServiceCollectionExtensions.BafModuleCatalog>();
        if (catalog is null)
        {
            // No modules registered
            return app;
        }

        // Configure middleware pipeline for each module
        foreach (var module in catalog.Modules)
        {
            module.Configure(app);
        }
        
        // Create a scoped DI context for initialization
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Initialize modules synchronously (safe during startup)
        foreach (var module in catalog.Modules)
        {
            module.InitializeAsync(serviceProvider, app.Lifetime.ApplicationStopping)
                .GetAwaiter()
                .GetResult(); // Blocking is OK here since startup is synchronous
        }

        return app;
    }
}