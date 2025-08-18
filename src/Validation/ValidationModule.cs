using System.Reflection;
using Avolutions.BAF.Core.Module.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.BAF.Core.Validation;

public class ValidationModule : IFeatureModule
{
    public void Register(IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly());
    }
}