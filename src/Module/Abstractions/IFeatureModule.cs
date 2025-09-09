using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Module.Abstractions;

/// <summary>
/// Defines a module in the Business Application Framework.
/// Modules can register services, configure the app pipeline and run initialization logic at startup.
/// </summary>
public interface IFeatureModule
{
    /// <summary>
    /// Register services for this module.
    /// </summary>
    void Register(IServiceCollection services);
    
    void Register(IServiceCollection services, Assembly[] assemblies)
        => Register(services);
    
    /// <summary>
    /// Optional pipeline configuration logic.
    /// </summary>
    void Configure(WebApplication app) {}
    
    /// <summary>
    /// Optional initialization logic that runs after the DI container is built.
    /// </summary>
    Task InitializeAsync(IServiceProvider services, CancellationToken ct = default) => Task.CompletedTask;
}