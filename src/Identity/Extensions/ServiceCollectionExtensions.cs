using Avolutions.BAF.Core.Identity.Models;
using Avolutions.BAF.Core.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        services.AddIdentityCore<User>(o =>
            {
                configure?.Invoke(o);
                o.SignIn.RequireConfirmedAccount = false;
                o.User.RequireUniqueEmail = false;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<BafDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }
}