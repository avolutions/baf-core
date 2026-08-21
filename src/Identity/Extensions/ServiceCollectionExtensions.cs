using Avolutions.Baf.Core.Identity.Abstractions;
using Avolutions.Baf.Core.Identity.Models;
using Avolutions.Baf.Core.Identity.Services;
using Avolutions.Baf.Core.Module.Extensions;
using Avolutions.Baf.Core.Persistence;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Identity.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> 
/// to register BAF modules and their services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBafIdentity(
        this IServiceCollection services,
        Action<IdentityOptions>? configure = null)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        var registrars = DiscoverPolicyRegistrars(services);

        services.AddAuthorization(options =>
        {
            foreach (var registrar in registrars)
            {
                registrar.Register(options);
            }
        });
        
        services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = false;
                configure?.Invoke(options);
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<BafDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }
    
    /// <summary>
    /// Finds all concrete types implementing <see cref="IPolicyRegistrar"/> in the
    /// assemblies scanned by <c>AddBafCore</c> and returns the created instances.
    /// </summary>
    private static List<IPolicyRegistrar> DiscoverPolicyRegistrars(IServiceCollection services)
    {
        if (services
                .FirstOrDefault(descriptor => descriptor.ServiceType == typeof(BafRegistry))?
                .ImplementationInstance is not BafRegistry registry)
        {
            throw new InvalidOperationException(
                "AddBafIdentity must be called after AddBafCore.");
        }

        var registrars = new List<IPolicyRegistrar>();

        foreach (var assembly in registry.ScannedAssemblies)
        {
            var registrarTypes = assembly.GetLoadableTypes()
                .Where(type => typeof(IPolicyRegistrar).IsAssignableFrom(type))
                .Where(type => type is { IsAbstract: false, IsInterface: false });

            foreach (var registrarType in registrarTypes)
            {
                if (Activator.CreateInstance(registrarType) is not IPolicyRegistrar registrar)
                {
                    continue;
                }

                registrars.Add(registrar);
            }
        }

        return registrars;
    }
}