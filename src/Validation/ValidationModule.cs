using System.Reflection;
using Avolutions.Baf.Core.Module.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Validation;

public class ValidationModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
    }

    public void Register(IServiceCollection services, Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
    }
}