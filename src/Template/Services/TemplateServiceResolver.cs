using Avolutions.Baf.Core.Template.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Avolutions.Baf.Core.Template.Services;

public class TemplateServiceResolver(IServiceProvider serviceProvider) : ITemplateServiceResolver
{
    public ITemplateService GetFieldExtractor(string extension) =>
        serviceProvider.GetKeyedService<ITemplateService>(Normalize(extension))
        ?? throw new NotSupportedException(
            $"No template service registered for '{extension}'.");
 
    public ITemplateService<TTemplate, TResult> GetTemplateService<TTemplate, TResult>(string extension)
    {
        var key = Normalize(extension);
 
        var service = serviceProvider.GetKeyedService<ITemplateService<TTemplate, TResult>>(key);
        if (service is not null)
        {
            return service;
        }
 
        // Distinguish "unknown extension" from "wrong type parameters" for a useful error.
        throw serviceProvider.GetKeyedService<ITemplateService>(key) is not null
            ? new InvalidOperationException(
                $"Template service for '{extension}' does not support " +
                $"{typeof(TTemplate).Name} -> {typeof(TResult).Name}.")
            : new NotSupportedException(
                $"No template service registered for '{extension}'.");
    }
 
    private static string Normalize(string extension) => extension.ToLowerInvariant();
}