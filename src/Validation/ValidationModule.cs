using System.Reflection;
using Avolutions.Baf.Core.Module.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Validation;

public class ValidationModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly());
    }
}