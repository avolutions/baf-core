using System.Reflection;
using Avolutions.Baf.Core.Template.Abstractions;
using Avolutions.Baf.Core.Template.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Template.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTemplateService<TService>(this IServiceCollection services)
        where TService : class, ITemplateService
    {
        var type = typeof(TService);

        var attrs = type.GetCustomAttributes<TemplateExtensionAttribute>().ToList();
        if (attrs.Count == 0)
        {
            throw new InvalidOperationException(
                $"{type.Name} is missing the [TemplateExtension] attribute.");
        }

        var closedGeneric = type.GetInterfaces().FirstOrDefault(i =>
                                i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(ITemplateService<,>))
                            ?? throw new InvalidOperationException(
                                $"{type.Name} does not implement ITemplateService<,>.");

        services.AddScoped<TService>();

        foreach (var attr in attrs)
        {
            var key = attr.Extension.ToLowerInvariant();

            services.AddKeyedScoped<ITemplateService>(key,
                (sp, _) => sp.GetRequiredService<TService>());
            services.AddKeyedScoped(closedGeneric, key,
                (sp, _) => sp.GetRequiredService<TService>());
        }

        return services;
    }
}