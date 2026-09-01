using System.Reflection;
using Avolutions.Baf.Core.Module.Abstractions;
using Avolutions.Baf.Core.Validation.Interceptors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Avolutions.Baf.Core.Validation;

public class ValidationModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.TryAddSingleton<ValidationInterceptor>();
    }

    public void Register(IServiceCollection services, Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
        
        Register(services);
    }
}