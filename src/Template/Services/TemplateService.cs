using System.Reflection;
using Avolutions.Baf.Core.Template.Abstractions;
using Avolutions.Baf.Core.Template.Attributes;

namespace Avolutions.Baf.Core.Template.Services;

public abstract class TemplateService<TTemplate, TResult> : ITemplateService<TTemplate, TResult>
{
    public Task<TResult> ApplyModelToTemplateAsync(TTemplate template, object model, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(model);

        var values = BuildValueDictionary(model);

        return ApplyValuesToTemplateAsync(template, values, ct);
    }
    
    protected abstract Task<TResult> ApplyValuesToTemplateAsync(
        TTemplate template,
        IDictionary<string, string> values,
        CancellationToken ct);
    
    protected Dictionary<string, string> BuildValueDictionary(object model)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var type = model.GetType();

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead)
            {
                continue;
            }

            var fieldName = GetDocumentFieldName(property);
            var value = property.GetValue(model);

            result[fieldName] = value?.ToString() ?? string.Empty;
        }

        return result;
    }
    
    protected static string GetDocumentFieldName(PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<TemplateFieldAttribute>();
        if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
        {
            return attribute.Name;
        }

        return property.Name;
    }
}