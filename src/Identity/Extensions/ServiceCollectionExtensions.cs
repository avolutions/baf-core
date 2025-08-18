using Avolutions.BAF.Core.Identity.Models;
using Avolutions.Baf.Core.Identity.Services;
using Avolutions.BAF.Core.Persistence;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Identity.Extensions;

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
}